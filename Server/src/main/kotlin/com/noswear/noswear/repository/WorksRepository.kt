package com.noswear.noswear.repository

import com.noswear.noswear.domain.Works
import org.springframework.data.jpa.repository.JpaRepository

interface WorksRepository : JpaRepository<Works, Int> {
    fun findByIdAndIsManagerTrue(id: Int): Works?
}