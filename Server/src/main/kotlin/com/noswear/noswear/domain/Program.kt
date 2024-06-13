package com.noswear.noswear.domain

import jakarta.persistence.Column
import jakarta.persistence.Entity
import jakarta.persistence.GeneratedValue
import jakarta.persistence.GenerationType
import jakarta.persistence.Id
import jakarta.persistence.OneToMany
import java.time.LocalDate
import java.util.*

@Entity
class Program(
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    val programId: Int? = null,
    @Column(nullable = false)
    val classId: String,
    @Column(nullable = false)
    val programName: String,
    @Column(nullable = false, columnDefinition = "DATE")
    val startDate: LocalDate,
    @Column(nullable = false, columnDefinition = "DATE")
    val endDate: LocalDate
) {
    @OneToMany(mappedBy = "joinsId.program")
    val joins = mutableListOf<Joins>()

    override fun equals(other: Any?): Boolean {
        if (other == null) {
            return false
        }

        if (this::class != other::class) {
            return false
        }

        return programId == (other as Program).programId
    }

    override fun hashCode() = Objects.hashCode(programId)
}