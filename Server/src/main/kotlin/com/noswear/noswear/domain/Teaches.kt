package com.noswear.noswear.domain

import jakarta.persistence.Column
import jakarta.persistence.Entity
import jakarta.persistence.Id
import java.util.*

@Entity
class Teaches (
    @Id
    val id: Int,
    @Column(nullable = false, unique = true)
    val cId: String
) {
    override fun equals(other: Any?): Boolean {
        if (other == null) {
            return false
        }

        if (this::class != other::class) {
            return false
        }

        return id == (other as Teaches).id
    }

    override fun hashCode() = Objects.hashCode(id)
}