package com.noswear.noswear.domain

import jakarta.persistence.Column
import jakarta.persistence.Entity
import jakarta.persistence.GeneratedValue
import jakarta.persistence.Id
import org.hibernate.annotations.GenericGenerator
import java.util.*

@Entity
class School(
    @Id
    @GeneratedValue(generator = "shortStringGenerator")
    @GenericGenerator(name = "shortStringGenerator", strategy = "com.noswear.noswear.utils.ShortStringGenerator")
    val schoolId: String? = null,
    @Column(nullable = false)
    val schoolName: String
) {
    override fun equals(other: Any?): Boolean {
        if (other == null) {
            return false
        }

        if (this::class != other::class) {
            return false
        }

        return schoolId == (other as School).schoolId
    }

    override fun hashCode() = Objects.hashCode(schoolId)
}