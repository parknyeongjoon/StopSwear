package com.noswear.noswear.repository

import com.noswear.noswear.domain.Program
import org.springframework.data.jpa.repository.JpaRepository
import org.springframework.data.jpa.repository.Query
import java.time.LocalDate

interface ProgramRepository : JpaRepository<Program, Int> {
    fun findByClassIdOrderByStartDateAsc(cId: String): List<Program>
    fun findByClassIdAndStartDateAfterOrderByStartDateAsc(cId: String, date: LocalDate): List<Program>
    fun findByClassIdAndProgramName(cId: String, programName: String): Program?
    fun existsByClassIdAndStartDateLessThanEqualAndEndDateGreaterThanEqual(cId: String, startDate: LocalDate, endDate: LocalDate): Boolean
    @Query("SELECT p " +
            "FROM Program p " +
            "WHERE p.classId = :cId AND p.startDate <= CURRENT_DATE AND p.endDate >= CURRENT_DATE")
    fun findCurrentProgramByClassId(cId: String): Program?
    @Query("SELECT p " +
            "FROM Program p " +
            "JOIN p.joins j " +
            "WHERE j.joinsId.user.id = :id AND p.startDate <= CURRENT_DATE AND p.endDate >= CURRENT_DATE")
    fun findCurrentProgramByUserId(id: Int): Program?

    @Query("SELECT COUNT(*) " +
            "FROM program p " +
            "INNER JOIN within w " +
            "ON p.class_id = w.c_id AND w.s_id = :schoolId AND p.start_date <= :date AND p.end_date >= :date",
        nativeQuery = true)
    fun countProceedingClass(schoolId: String, date: LocalDate): Int
}