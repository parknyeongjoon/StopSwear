package com.noswear.noswear.dto

import com.noswear.noswear.domain.Program
import java.time.LocalDate

data class ProgramDto(
    val programName: String,
    val startDate: LocalDate,
    val endDate: LocalDate
) {
    companion object {
        fun of(program: Program): ProgramDto {
            return ProgramDto(
                programName = program.programName,
                startDate = program.startDate,
                endDate = program.endDate
            )
        }
    }
}
