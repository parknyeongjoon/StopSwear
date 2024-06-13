package com.noswear.noswear.repository

import com.noswear.noswear.domain.WordCount
import com.noswear.noswear.domain.WordCountId
import org.springframework.data.jpa.repository.JpaRepository
import org.springframework.data.jpa.repository.Query
import java.time.LocalDate

interface WordCountRepository : JpaRepository<WordCount, WordCountId> {
    fun findByWordCountIdIdAndWordCountIdDateOrderByCountDesc(id: Int, date: LocalDate): List<WordCount>
    @Query("SELECT word AS word, SUM(count) AS count " +
            "FROM word_count " +
            "WHERE id = :id AND date >= :startDate AND date <= :endDate " +
            "GROUP BY word " +
            "ORDER BY count DESC",
        nativeQuery = true)
    fun findProgramWordCount(id: Int, startDate: LocalDate, endDate: LocalDate): List<WordCountResultVo>
    @Query("SELECT word AS word, SUM(count) AS count " +
            "FROM word_count w " +
            "JOIN joins j " +
            "WHERE w.id = j.id AND j.program_id = :programId AND w.date >= :startDate AND w.date <= :endDate " +
            "GROUP BY w.word " +
            "ORDER BY count DESC",
        nativeQuery = true)
    fun findGroupWordCount(programId: Int, startDate: LocalDate, endDate: LocalDate): List<WordCountResultVo>

    @Query("SELECT word " +
            "FROM word_count " +
            "WHERE id = :id AND date = :date " +
            "ORDER BY count DESC " +
            "LIMIT 1",
        nativeQuery = true)
    fun findFirstMostUsedWordInDay(id: Int, date: LocalDate): String?

    @Query("SELECT word " +
            "FROM (SELECT word AS word, SUM(count) AS count " +
            "FROM word_count " +
            "WHERE id = :id AND date >= :startDate AND date <= :endDate " +
            "GROUP BY word " +
            "ORDER BY count DESC " +
            "LIMIT 1)",
        nativeQuery = true)
    fun findFirstMostUsedWordInProgram(id: Int, startDate: LocalDate, endDate: LocalDate): String?

    interface WordCountResultVo {
        fun getWord(): String
        fun getCount(): Int
    }
}