package com.noswear.noswear.domain

import jakarta.persistence.Column
import jakarta.persistence.Entity
import jakarta.persistence.Id
import java.util.*

@Entity
class Within(
    @Id
    val cId: String,
    @Column(nullable = false)
    val sId: String
) {
    override fun equals(other: Any?): Boolean {
        if (other == null) {
            return false
        }

        if (this::class != other::class) {
            return false
        }

        return cId == (other as Within).cId
    }

    override fun hashCode() = Objects.hashCode(cId)
}