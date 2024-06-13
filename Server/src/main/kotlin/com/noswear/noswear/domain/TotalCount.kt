package com.noswear.noswear.domain

import jakarta.persistence.Column
import jakarta.persistence.Embeddable
import jakarta.persistence.EmbeddedId
import jakarta.persistence.Entity
import java.io.Serializable
import java.time.LocalDate
import java.util.*

@Entity
class TotalCount(
    @EmbeddedId
    val totalCountId: TotalCountId,
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

        return totalCountId == (other as TotalCount).totalCountId
    }

    override fun hashCode() = Objects.hashCode(totalCountId)
}

@Embeddable
data class TotalCountId(
    @Column(nullable = false)
    val id: Int,
    @Column(nullable = false, columnDefinition = "DATE")
    val date: LocalDate,
    @Column(nullable = false)
    val time: Int
) : Serializable