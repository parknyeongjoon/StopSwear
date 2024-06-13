package com.noswear.noswear.dto

import com.noswear.noswear.repository.WordCountRepository

data class WordCountResult(
    val word: String,
    val count: Int
) {
    companion object {
        fun of(wordCountResultVo: WordCountRepository.WordCountResultVo): WordCountResult {
            return WordCountResult(
                word = wordCountResultVo.getWord(),
                count = wordCountResultVo.getCount()
            )
        }
    }
}
