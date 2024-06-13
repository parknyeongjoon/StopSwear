package com.noswear.noswear.dto

import com.noswear.noswear.domain.Program
import com.noswear.noswear.repository.TotalCountRepository
import java.time.DayOfWeek
import java.time.LocalDate

data class DailyCountResponse(
    val raw: List<Map<String, Any>>,
    val min: Int,
    val max: Int,
    val avg: Double,
    val sum: Int
) {
    companion object {
        fun of(dailyCountVoList: List<TotalCountRepository.DailyCountVo>, program: Program, end: LocalDate?): DailyCountResponse {
            var days = 0
            val raw = buildList {
                var date = program.startDate
                while (date <= (end ?: program.endDate)) {
                    val count = dailyCountVoList.find { it.date == date }?.count
                        ?: 0
                    add(
                        mapOf(
                            "date" to date,
                            "count" to count
                        )
                    )
                    do {
                        date = date.plusDays(1)
                    } while (date.dayOfWeek == DayOfWeek.SUNDAY || date.dayOfWeek == DayOfWeek.SATURDAY)
                    days += 1
                }
            }
            val sum = dailyCountVoList.sumOf { it.count }
            return DailyCountResponse(
                raw = raw,
                min = if (dailyCountVoList.size == days)
                    dailyCountVoList.minOf { it.count }
                else
                    0,
                max = if (dailyCountVoList.isNotEmpty())
                    dailyCountVoList.maxOf { it.count }
                else
                    0,
                avg = sum.toDouble() / days,
                sum = sum
            )
        }
    }
}