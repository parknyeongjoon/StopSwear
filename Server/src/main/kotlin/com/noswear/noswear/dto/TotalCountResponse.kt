package com.noswear.noswear.dto

import com.noswear.noswear.domain.TotalCount

data class TotalCountResponse (
    val raw: List<Map<String, Int>>,
    val min: Int,
    val max: Int,
    val avg: Double
) {
    companion object {
        fun of(totalCountList: List<TotalCount>): TotalCountResponse {
            return TotalCountResponse(
                raw = buildList {
                    for (hour in 9..17) {
                        val count = totalCountList.find { it.totalCountId.time == hour }?.count
                            ?: 0
                        add(mapOf(
                            "hour" to hour,
                            "count" to count
                        ))
                    }
                },
                min = if (totalCountList.isNotEmpty())
                    totalCountList.minOf { it.count }
                else
                    0,
                max = if (totalCountList.isNotEmpty())
                    totalCountList.maxOf { it.count }
                else
                    0,
                avg = (totalCountList.sumOf { it.count }).toDouble() / 9
            )
        }
    }
}