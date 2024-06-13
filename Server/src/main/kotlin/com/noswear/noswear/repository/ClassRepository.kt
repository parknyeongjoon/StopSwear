package com.noswear.noswear.repository

import com.noswear.noswear.domain.Class
import org.springframework.data.jpa.repository.JpaRepository

interface ClassRepository : JpaRepository<Class, String> {
    fun existsByClassId(classId: String): Boolean
}