import argparse
import os
import sys
from typing import Dict

import numpy as np
import torch

from RawNet3 import RawNet3
from RawNetBasicBlock import Bottle2neck

from pyannote.audio import Pipeline
import librosa

from scipy.spatial.distance import cdist

from sttModel import SttModel

def main(args: Dict) -> None:
    model_embadding = RawNet3(
        Bottle2neck,
        model_scale=8,
        context=True,
        summed=True,
        encoder_type="ECA",
        nOut=256,
        out_bn=False,
        sinc_stride=10,
        log_sinc=True,
        norm_sinc="mean",
        grad_mult=1,
    )
    gpu = False

    model_embadding.load_state_dict(
        torch.load(
            "./models/verification/model.pt",
            map_location=lambda storage, loc: storage,
        )["model"]
    )
    model_embadding.eval()

    pipeline = Pipeline.from_pretrained("pyannote/speaker-diarization-3.1", use_auth_token="[token]")

    model_stt = SttModel("./models/stt")

    if torch.cuda.is_available():
        model_embadding = model_embadding.to("cuda")
        pipeline.to(torch.device("cuda"))
        gpu = True
    
    #diarization using pipeline
    waveform, sample_rate = librosa.load(args.input2, sr=16000)
    waveform_tensor = torch.Tensor(waveform).unsqueeze(0)
    diarization = pipeline({"waveform": waveform_tensor, "sample_rate": sample_rate})
    
    #create diarization audio list 
    audio_list = audio_diarization(waveform, sample_rate, diarization)
    origin_waveform, origin_sample_rate = librosa.load(args.input1, sr=16000)

    origin_emb = extract_speaker_embd(
        model_embadding,
        audio=origin_waveform,
        sample_rate=origin_sample_rate,
        n_samples=48000,
        gpu=gpu,
    ).mean(0)
    origin_matrix = np.array([origin_emb.detach().cpu().numpy()])

    pass_list = []
    for i in range(len(audio_list)):
        other_emb = extract_speaker_embd(
            model_embadding,
            audio=audio_list[i],
            sample_rate=sample_rate,
            n_samples=48000,
            gpu=gpu,
        ).mean(0)
        other_matrix = np.array([other_emb.detach().cpu().numpy()])

        distance = cdist(origin_matrix, other_matrix, metric="cosine")
        if distance <= 0.55:
            pass_list.append(audio_list[i])
        
    result = []
    for i in range(len(pass_list)):
        trans_list = model_stt.predict(pass_list[i])
        result.extend(trans_list)
    return result

def extract_speaker_embd(
    model, audio, sample_rate: int, n_samples: int, gpu: bool = False
) -> np.ndarray:
    if len(audio.shape) > 1:
        raise ValueError(
            f"RawNet3 supports mono input only. Input data has a shape of {audio.shape}."
        )

    if sample_rate != 16000:
        raise ValueError(
            f"RawNet3 supports 16k sampling rate only. Input data's sampling rate is {sample_rate}."
        )

    if (
        len(audio) < n_samples
    ):  # RawNet3 was trained using utterances of 3 seconds
        shortage = n_samples - len(audio) + 1
        audio = np.pad(audio, (0, shortage), "wrap")

    audios = []
    startframe = np.linspace(0, len(audio) - n_samples)
    for asf in startframe:
        audios.append(audio[int(asf) : int(asf) + n_samples])

    audios = torch.from_numpy(np.stack(audios, axis=0).astype(np.float32))
    if gpu:
        audios = audios.to("cuda")
    with torch.no_grad():
        output = model(audios)

    return output

def audio_diarization(audio_array, sample_rate, diarization):
    pre_speaker = "SPEAKER_00"
    source_list = []
    start = 0
    end = 0
    segment = None
    for turn, _, speaker in diarization.itertracks(yield_label=True):
        '''
        start = turn.start
        end = turn.end
        source_list.append(audio_array[int(sample_rate * start):int(sample_rate * end)])
        '''
        if pre_speaker is speaker:
            end = turn.end
            segment = audio_array[int(sample_rate * start):int(sample_rate * end)]
        else:
            if segment is not None:
                source_list.append(segment)
            pre_speaker = speaker
            start = turn.start
            end = turn.end
            segment = audio_array[int(sample_rate * start):int(sample_rate * end)]
    source_list.append(segment)

    return source_list

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="RawNet3 inference")
    parser.add_argument(
        "--input1",
        type=str,
        default="",
        help="Input file1(user audio) for verification. Required when 'verification' is True",
    )
    parser.add_argument(
        "--input2",
        type=str,
        default="",
        help="Input file2(record file) for verification. Required when 'verification' is True",
    )
    args = parser.parse_args()

    assert args.input1 != ""
    assert args.input2 != ""
    sys.exit(main(args))
