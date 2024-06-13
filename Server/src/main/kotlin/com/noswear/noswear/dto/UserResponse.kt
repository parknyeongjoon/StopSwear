package com.noswear.noswear.dto

import com.noswear.noswear.domain.User

data class UserResponse(
    val id: Int,
    val name: String,
    val email: String
) {
    companion object {
        fun of(user: User): UserResponse {
            return UserResponse(
                user.id!!,
                user.name,
                user.email
            )
        }
    }

}
