package com.noswear.noswear.domain

import jakarta.persistence.Column
import jakarta.persistence.Entity
import jakarta.persistence.Id
import java.util.*

@Entity
class Works (
    @Id
    val id: Int,
    @Column(nullable = false)
    val sId: String,
    @Column(nullable = false)
    val isManager: Boolean
) {
    override fun equals(other: Any?): Boolean {
        if (other == null) {
            return false
        }

        if (this::class != other::class) {
            return false
        }

        return id == (other as Works).id
    }

    override fun hashCode() = Objects.hashCode(id)
}