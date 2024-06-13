package com.noswear.noswear.dto

import com.fasterxml.jackson.annotation.JsonIgnoreProperties

@JsonIgnoreProperties(ignoreUnknown = true)
data class TeacherDto (
    val email: String,
    val password: String,
    val name: String,
    val create: Boolean,
    val schoolName: String?,
    val schoolId: String?,
    val className: String
)