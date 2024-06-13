package com.noswear.noswear.repository

import com.noswear.noswear.domain.Within
import org.springframework.data.jpa.repository.JpaRepository

interface WithinRepository : JpaRepository<Within, String> {
}