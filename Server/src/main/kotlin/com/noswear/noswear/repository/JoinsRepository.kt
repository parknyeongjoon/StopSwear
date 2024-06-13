package com.noswear.noswear.repository

import com.noswear.noswear.domain.Joins
import com.noswear.noswear.domain.JoinsId
import com.noswear.noswear.domain.User
import org.springframework.data.jpa.repository.JpaRepository
import org.springframework.data.jpa.repository.Query
import java.time.LocalDate

interface JoinsRepository : JpaRepository<Joins, JoinsId> {
    fun findByJoinsIdUserIdOrderByJoinsIdProgramStartDateAsc(id: Int): List<Joins>
    fun countByJoinsIdProgramProgramId(id: Int): Int
    fun findByJoinsIdProgramClassIdAndJoinsIdProgramProgramNameOrderByJoinsIdUserNameAsc(cId: String, programName: String): List<Joins>
    @Query("WITH ranked AS( " +
            "SELECT RANK() OVER (ORDER BY COALESCE(SUM(count), 0) ASC) AS rank, j.id " +
            "FROM joins j " +
            "LEFT OUTER JOIN " +
            "(SELECT *" +
            "FROM total_count " +
            "WHERE date >= :startDate AND date <= :endDate) t ON t.id = j.id " +
            "WHERE j.program_id = :programId GROUP BY j.id) " +
            "SELECT u.id, u.email, u.password, u.name, u.role " +
            "FROM users u " +
            "JOIN ranked r " +
            "ON u.id = r.id " +
            "ORDER BY r.rank DESC",
        nativeQuery = true)

    fun findProgramStudentsSorted(programId: Int, startDate: LocalDate, endDate: LocalDate): List<UserVo>

    interface UserVo {
        val id: Int
        val email: String
        val password: String
        val name: String
        val role: String
    }
}