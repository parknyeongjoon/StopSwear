package com.noswear.noswear.repository

import com.noswear.noswear.domain.Teaches
import org.springframework.data.jpa.repository.JpaRepository

interface TeachesRepository : JpaRepository<Teaches, Int> {
}