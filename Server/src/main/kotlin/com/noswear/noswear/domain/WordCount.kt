package com.noswear.noswear.domain

import jakarta.persistence.Column
import jakarta.persistence.Embeddable
import jakarta.persistence.EmbeddedId
import jakarta.persistence.Entity
import java.io.Serializable
import java.time.LocalDate
import java.util.*

@Entity
class WordCount(
    @EmbeddedId
    val wordCountId: WordCountId,
    @Column(nullable = false)
    var count: Int
) {
    override fun equals(other: Any?): Boolean {
        if (other == null) {
            return false
        }

        if (this::class != other::class) {
            return false
        }

        return wordCountId == (other as WordCount).wordCountId
    }

    override fun hashCode() = Objects.hashCode(wordCountId)
}

@Embeddable
data class WordCountId(
    @Column(nullable = false)
    val id: Int,
    @Column(nullable = false, columnDefinition = "DATE")
    val date: LocalDate,
    @Column(nullable = false)
    val word: String
) : Serializable