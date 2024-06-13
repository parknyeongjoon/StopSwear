package com.noswear.noswear.repository

import com.noswear.noswear.domain.Belongs
import org.springframework.data.jpa.repository.JpaRepository

interface BelongsRepository : JpaRepository<Belongs, Int> {
    fun findByClassId(cId: String): List<Belongs>
}