package com.noswear.noswear.domain

import jakarta.persistence.Embeddable
import jakarta.persistence.EmbeddedId
import jakarta.persistence.Entity
import jakarta.persistence.JoinColumn
import jakarta.persistence.ManyToOne
import java.io.Serializable
import java.util.*

@Entity
class Joins(
    @EmbeddedId
    val joinsId: JoinsId
) {
    override fun equals(other: Any?): Boolean {
        if (other == null) {
            return false
        }

        if (this::class != other::class) {
            return false
        }

        return joinsId == (other as Joins).joinsId
    }

    override fun hashCode() = Objects.hashCode(joinsId)
}

@Embeddable
data class JoinsId(
    @ManyToOne
    @JoinColumn(name = "id")
    val user: User,
    @ManyToOne
    @JoinColumn(name = "program_id")
    val program: Program
) : Serializable