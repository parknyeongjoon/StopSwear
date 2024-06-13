package com.noswear.noswear.service

import com.noswear.noswear.domain.TotalCount
import com.noswear.noswear.domain.TotalCountId
import com.noswear.noswear.domain.WordCount
import com.noswear.noswear.domain.WordCountId
import com.noswear.noswear.dto.DailyCountResponse
import com.noswear.noswear.dto.SendDto
import com.noswear.noswear.repository.*
import com.noswear.noswear.utils.ProfanityUtil
import mu.KotlinLogging
import org.springframework.security.core.userdetails.UsernameNotFoundException
import org.springframework.stereotype.Service
import java.io.BufferedReader
import java.io.File
import java.io.InputStreamReader
import java.time.LocalDate
import java.time.LocalDateTime
import java.util.*

private val logger = KotlinLogging.logger {}

@Service
class StatisticsService(
    private val wordCountRepository: WordCountRepository,
    private val totalCountRepository: TotalCountRepository,
    private val userRepository: UserRepository,
    private val belongsRepository: BelongsRepository,
    private val programRepository: ProgramRepository,
    private val teachesRepository: TeachesRepository,
    private val withinRepository: WithinRepository
) {
    fun analyzeProfanity(email: String, sendDto: SendDto): Map<String, Int> {
        val result = HashMap<String, Int>()

        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")
        val id = user.id!!

        val date = sendDto.dateTime.toLocalDate()
        val hour = sendDto.dateTime.hour

        val path = "./temp_files/${user.id}_${sendDto.dateTime}.m4a"
        val byteArray = Base64.getDecoder().decode(sendDto.data)
        val file = File(path)
        file.writeBytes(byteArray)

        val badWords = ProfanityUtil.getBadWords()

        var count = 0

        logger.info("start analyzing $path by $email(${user.name})")
        val processBuilder = ProcessBuilder("python3", "./capstone_ai/run.py", "--input1", "./voice_files/${user.id!!}.m4a", "--input2", path)
        val process = processBuilder.start()
        BufferedReader(InputStreamReader(process.inputStream))
            .forEachLine { line ->
                logger.info(line)
                line.split(" ").map { split ->
                    badWords.forEach { word ->
                        if (split.contains(word)) {
                            val wordCountId = WordCountId(
                                id = id,
                                word = word,
                                date = date
                            )

                            wordCountRepository.findById(wordCountId)
                                .map { wordCount ->
                                    wordCount.count += 1
                                    wordCountRepository.save(wordCount)
                                }.orElseGet {
                                    wordCountRepository.save(
                                        WordCount(
                                            wordCountId = wordCountId,
                                            count = 1
                                        )
                                    )
                                }

                            if (result.containsKey(word)) {
                                result[word] = result[word]!! + 1
                            } else {
                                result[word] = 1
                            }

                            count++
                        }
                    }
                }
            }

        if (count > 0) {
            val totalCountId = TotalCountId(
                id = id,
                date = date,
                time = hour
            )

            totalCountRepository.findById(totalCountId)
                .map { totalCount ->
                    totalCount.count += count
                    totalCountRepository.save(totalCount)
                }.orElseGet {
                    totalCountRepository.save(
                        TotalCount(
                            totalCountId = totalCountId,
                            count = count
                        )
                    )
                }
        }

        file.delete()

        logger.info("analyzing completed $path by $email(${user.name})")

        return result
    }

    fun getTotalCount(email: String, date: LocalDate): List<TotalCount> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")
        val id = user.id!!

        return totalCountRepository.findByTotalCountIdIdAndTotalCountIdDate(id, date)
    }

    fun getTotalCountByTeacher(email: String, studentId: Int, date: LocalDate): List<TotalCount> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")
        val id = user.id!!

        return totalCountRepository.findByTotalCountIdIdAndTotalCountIdDate(studentId, date)
    }

    fun getWordCount(email: String, date: LocalDate): List<WordCount> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")
        val id = user.id!!

        return wordCountRepository.findByWordCountIdIdAndWordCountIdDateOrderByCountDesc(id, date)
    }

    fun getWordCountByTeacher(email: String, studentId: Int, date: LocalDate): List<WordCount> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")
        val id = user.id!!

        return wordCountRepository.findByWordCountIdIdAndWordCountIdDateOrderByCountDesc(studentId, date)
    }

    fun getProgramWordCount(email: String, programName: String): List<WordCountRepository.WordCountResultVo> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")
        val id = user.id!!

        val cId = belongsRepository.findById(user.id!!)
            .orElseThrow()
            .classId

        val program = programRepository.findByClassIdAndProgramName(cId, programName)
            ?: throw Exception()

        return wordCountRepository.findProgramWordCount(id, program.startDate, program.endDate)
    }

    fun getProgramWordCountByTeacher(email: String, studentId: Int, programName: String): List<WordCountRepository.WordCountResultVo> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = belongsRepository.findById(studentId)
            .orElseThrow()
            .classId

        val program = programRepository.findByClassIdAndProgramName(cId, programName)
            ?: throw Exception()

        return wordCountRepository.findProgramWordCount(studentId, program.startDate, program.endDate)
    }

    fun getGroupWordCount(email: String, programName: String): List<WordCountRepository.WordCountResultVo> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = if (user.role == "STUDENT") {
            belongsRepository.findById(user.id!!)
                .orElseThrow()
                .classId
        } else {
            teachesRepository.findById(user.id!!)
                .orElseThrow()
                .cId
        }
        val program = programRepository.findByClassIdAndProgramName(cId, programName)
            ?: throw Exception()

        return wordCountRepository.findGroupWordCount(program.programId!!, program.startDate, program.endDate)
    }

    fun getDailyCount(email: String, programName: String, date: LocalDate?): DailyCountResponse {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = belongsRepository.findById(user.id!!)
                .orElseThrow()
                .classId

        val program = programRepository.findByClassIdAndProgramName(cId, programName)
            ?: throw Exception()

        val dailyCount = totalCountRepository.findDailyCount(user.id!!, program.startDate, date ?: program.endDate)
        return DailyCountResponse.of(dailyCount, program, date)
    }

    fun getDailyCountByTeacher(email: String, studentId: Int, programName: String, date: LocalDate?): DailyCountResponse {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = belongsRepository.findById(studentId)
            .orElseThrow()
            .classId

        val program = programRepository.findByClassIdAndProgramName(cId, programName)
            ?: throw Exception()

        val dailyCount = totalCountRepository.findDailyCount(studentId, program.startDate, date ?: program.endDate)
        return DailyCountResponse.of(dailyCount, program, date)
    }

    fun getGroupDailyCount(email: String, programName: String, date: LocalDate?): DailyCountResponse {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = if (user.role == "STUDENT") {
            belongsRepository.findById(user.id!!)
                .orElseThrow()
                .classId
        } else {
            teachesRepository.findById(user.id!!)
                .orElseThrow()
                .cId
        }
        val program = programRepository.findByClassIdAndProgramName(cId, programName)
            ?: throw Exception()

        val dailyCount = totalCountRepository.findGroupDailyCount(program.programId!!, program.startDate, date ?: program.endDate)
        return DailyCountResponse.of(dailyCount, program, date)
    }

    fun getMostUsedWordInDay(email: String, date: LocalDate): String? {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        return wordCountRepository.findFirstMostUsedWordInDay(user.id!!, date)
    }

    fun getMostUsedWordInDayByTeacher(email: String, studentId: Int, date: LocalDate): String? {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        return wordCountRepository.findFirstMostUsedWordInDay(studentId, date)
    }

    fun getMostUsedWordInProgram(email: String, programName: String): String? {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = belongsRepository.findById(user.id!!)
            .orElseThrow()
            .classId

        val program = programRepository.findByClassIdAndProgramName(cId, programName)
            ?: throw Exception()

        return wordCountRepository.findFirstMostUsedWordInProgram(user.id!!, program.startDate, program.endDate)
    }

    fun getMostUsedWordInProgramByTeacher(email: String, studentId: Int, programName: String): String? {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = belongsRepository.findById(studentId)
            .orElseThrow()
            .classId

        val program = programRepository.findByClassIdAndProgramName(cId, programName)
            ?: throw Exception()

        return wordCountRepository.findFirstMostUsedWordInProgram(studentId, program.startDate, program.endDate)
    }

    fun getMyRank(email: String, programName: String, date: LocalDate?): Int {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = belongsRepository.findById(user.id!!)
            .orElseThrow()
            .classId

        val program = programRepository.findByClassIdAndProgramName(cId, programName)
            ?: throw Exception()

        return totalCountRepository.findRankByIdAndProgramIdAndStartDateAndEndDate(
            user.id!!,
            program.programId!!,
            program.startDate,
            date ?: program.endDate
        )
    }

    fun getStudentRankByTeacher(email: String, studentId: Int, programName: String, date: LocalDate?): Int {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = belongsRepository.findById(studentId)
            .orElseThrow()
            .classId

        val program = programRepository.findByClassIdAndProgramName(cId, programName)
            ?: throw Exception()

        return totalCountRepository.findRankByIdAndProgramIdAndStartDateAndEndDate(
            studentId,
            program.programId!!,
            program.startDate,
            date ?: program.endDate
        )
    }

    fun getClassRank(email: String, date: LocalDate): Int {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = if (user.role == "STUDENT") {
            belongsRepository.findById(user.id!!)
                .orElseThrow()
                .classId
        } else {
            teachesRepository.findById(user.id!!)
                .orElseThrow()
                .cId
        }

        val sId = withinRepository.findById(cId)
            .orElseThrow()
            .sId

        return totalCountRepository.findClassRankBySchoolIdAndClassIdAndDate(
            schoolId = sId,
            classId = cId,
            date = date
        )
    }
}