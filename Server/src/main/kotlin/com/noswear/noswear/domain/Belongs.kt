package com.noswear.noswear.domain

import jakarta.persistence.*
import java.util.*

@Entity
class Belongs (
    @OneToOne(fetch = FetchType.EAGER)
    @MapsId
    @JoinColumn(name = "id")
    val user: User,
    @Column(nullable = false)
    val classId: String
) {
    @Id
    val id: Int? = null

    override fun equals(other: Any?): Boolean {
        if (other == null) {
            return false
        }

        if (this::class != other::class) {
            return false
        }

        return id == (other as Belongs).id
    }

    override fun hashCode() = Objects.hashCode(id)
}