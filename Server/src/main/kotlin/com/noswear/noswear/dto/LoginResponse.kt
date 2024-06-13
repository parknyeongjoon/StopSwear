package com.noswear.noswear.dto

data class LoginResponse (
    val token: String,
    val expiresIn: Long,
    val name: String,
    val role: String
)