package com.noswear.noswear.repository

import com.noswear.noswear.domain.TotalCount
import com.noswear.noswear.domain.TotalCountId
import org.springframework.data.jpa.repository.JpaRepository
import org.springframework.data.jpa.repository.Query
import java.time.LocalDate

interface TotalCountRepository : JpaRepository<TotalCount, TotalCountId> {
    fun findByTotalCountIdIdAndTotalCountIdDate(id: Int, date: LocalDate): List<TotalCount>
    @Query("WITH ranked AS( " +
            "SELECT RANK() OVER (ORDER BY COALESCE(SUM(count), 0) ASC) AS rank, j.id " +
            "FROM joins j " +
            "LEFT OUTER JOIN " +
            "(SELECT *" +
            "FROM total_count " +
            "WHERE date >= :startDate AND date <= :endDate) t ON t.id = j.id " +
            "WHERE j.program_id = :programId GROUP BY j.id) " +
            "SELECT rank FROM ranked WHERE id=:id",
        nativeQuery = true)
    fun findRankByIdAndProgramIdAndStartDateAndEndDate(id: Int, programId: Int, startDate: LocalDate, endDate: LocalDate): Int
    @Query("WITH ranked AS( " +
            "SELECT RANK() OVER (ORDER BY COALESCE(SUM(count), 0) ASC) AS rank, b.class_id " +
            "FROM within w " +
            "LEFT OUTER JOIN belongs b " +
            "ON w.c_id = b.class_id AND w.s_id = :schoolId " +
            "LEFT OUTER JOIN total_count t " +
            "ON t.id = b.id AND t.date=:date " +
            "WHERE EXISTS( " +
            "SELECT 1 " +
            "FROM program p " +
            "WHERE p.class_id = w.c_id AND :date BETWEEN p.start_date AND p.end_date) " +
            "GROUP BY b.class_id) " +
            "SELECT rank FROM ranked WHERE class_id = :classId",
        nativeQuery = true)
    fun findClassRankBySchoolIdAndClassIdAndDate(schoolId: String, classId: String, date: LocalDate): Int

    @Query("SELECT date, SUM(count) AS count " +
            "FROM total_count " +
            "WHERE id = :id AND date >= :startDate AND date <= :endDate " +
            "GROUP BY date",
        nativeQuery = true)
    fun findDailyCount(id: Int, startDate: LocalDate, endDate: LocalDate): List<DailyCountVo>
    @Query("SELECT date, SUM(count) AS count " +
            "FROM total_count t " +
            "JOIN joins j " +
            "WHERE t.id = j.id AND j.program_id = :programId AND t.date >= :startDate AND t.date <= :endDate " +
            "GROUP BY t.date ",
        nativeQuery = true)
    fun findGroupDailyCount(programId: Int, startDate: LocalDate, endDate: LocalDate): List<DailyCountVo>

    interface DailyCountVo {
        val date: LocalDate
        val count: Int
    }
}