from transformers import WhisperProcessor, WhisperForConditionalGeneration
import torch
import math

class SttModel():
    def __init__(self, model_path):
        self.processor = WhisperProcessor.from_pretrained(model_path)
        self.model = WhisperForConditionalGeneration.from_pretrained(model_path)
        self.model.to("cuda")
        self.model.eval().cuda()

    def split_audio_data(self, audio_array, sample_rate):
        split_sec = 30
        list_of_audio_part = []
        size_of_range = audio_array.shape[0]/( sample_rate * split_sec )
        size_of_range_int = math.floor( size_of_range )
        
        if size_of_range < 1:
            list_of_audio_part.append(audio_array)
            return list_of_audio_part
        
        for i in range(size_of_range_int):
            list_of_audio_part.append(audio_array[sample_rate * split_sec * i:sample_rate * split_sec * (i+1)])
        
        list_of_audio_part.append(audio_array[sample_rate * split_sec * size_of_range_int:])
        return list_of_audio_part
    
    def predict(self, speech):
        array = self.split_audio_data(speech, 16000)
        transcription_list = []
        for i in range(len(array)):
            input_features = self.processor(array[i], sampling_rate=16000, return_tensors="pt").input_features
            input_tensor = torch.tensor(input_features).to("cuda")
            predicted_ids = self.model.generate(input_tensor)
            transcription_list.append(self.processor.batch_decode(predicted_ids, skip_special_tokens=True))
        return transcription_list