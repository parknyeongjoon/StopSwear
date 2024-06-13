import argparse
import os
import sys
from typing import Dict

import numpy as np
import torch

from RawNet3 import RawNet3
from RawNetBasicBlock import Bottle2neck

import librosa

from scipy.spatial.distance import cdist

import json
import os
import pandas as pd
from tqdm import tqdm
from pathlib import Path

origin_audio_path = Path('./data/2021-12-13/0000/B0001-0000F1012-101201_0-00369848.wav')
audio_path = Path('./data/2021-12-13/0000')
audio_list = os.listdir(audio_path)

def main() -> None:
    starter, ender = torch.cuda.Event(enable_timing=True), torch.cuda.Event(enable_timing=True)
    repetitions = 300
    timings=np.zeros((repetitions,1))

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

    if torch.cuda.is_available():
        model_embadding = model_embadding.to("cuda")
        gpu = True
    
    pass_count = 0
    count = 0
    timings = []
    for i in tqdm(range(len(audio_list))):
        origin_audio, origin_sample_rate = librosa.load(os.path.join(audio_path, audio_list[i]), sr=16000)
        starter.record()
        origin_emb = extract_speaker_embd(
            model_embadding,
            audio=origin_audio,
            sample_rate=origin_sample_rate,
            n_samples=48000,
            gpu=gpu,
        ).mean(0)
        ender.record()
        torch.cuda.synchronize()
        curr_time = starter.elapsed_time(ender)
        timings.append(curr_time)
        count += 1
        origin_matrix = np.array([origin_emb.detach().cpu().numpy()])

        for j in tqdm(range(len(audio_list))):
            if i == j:
                continue
            other_audio, other_sample_rate = librosa.load(os.path.join(audio_path, audio_list[j]), sr=16000)
            starter.record()
            other_emb = extract_speaker_embd(
                model_embadding,
                audio=other_audio,
                sample_rate=other_sample_rate,
                n_samples=48000,
                gpu=gpu,
            ).mean(0)
            ender.record()
            torch.cuda.synchronize()
            curr_time = starter.elapsed_time(ender)
            timings.append(curr_time)
            count += 1
            other_matrix = np.array([other_emb.detach().cpu().numpy()])
            distance = cdist(origin_matrix, other_matrix, metric="cosine")
            if distance <= 0.55:
                pass_count += 1
        time = 0
        for i in range(len(timings)):
            time += timings[i]
        print(f"mean time : {time / len(timings)}")
        print(f"pass count : {pass_count}")


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

main()