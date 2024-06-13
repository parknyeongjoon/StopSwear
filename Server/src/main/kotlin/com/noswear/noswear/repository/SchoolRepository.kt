package com.noswear.noswear.repository

import com.noswear.noswear.domain.School
import org.springframework.data.jpa.repository.JpaRepository

interface SchoolRepository : JpaRepository<School, String> {
    fun existsBySchoolId(schoolId: String): Boolean
}