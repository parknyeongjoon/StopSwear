package com.noswear.noswear.service

import com.noswear.noswear.domain.Joins
import com.noswear.noswear.domain.JoinsId
import com.noswear.noswear.domain.Program
import com.noswear.noswear.domain.User
import com.noswear.noswear.dto.JoinDto
import com.noswear.noswear.dto.ProgramDto
import com.noswear.noswear.dto.VoiceDto
import com.noswear.noswear.repository.*
import org.springframework.security.core.userdetails.UsernameNotFoundException
import org.springframework.stereotype.Service
import java.io.File
import java.lang.Exception
import java.time.LocalDate
import java.util.*

@Service
class ManageService(
    private val programRepository: ProgramRepository,
    private val userRepository: UserRepository,
    private val teachesRepository: TeachesRepository,
    private val worksRepository: WorksRepository,
    private val joinsRepository: JoinsRepository,
    private val belongsRepository: BelongsRepository,
    private val classRepository: ClassRepository,
    private val withinRepository: WithinRepository
) {
    fun createProgram(email: String, programDto: ProgramDto): Program {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val classId = teachesRepository.findById(user.id!!)
            .orElseThrow()
            .cId

        val program = Program(
            classId = classId,
            programName = programDto.programName,
            startDate = programDto.startDate,
            endDate = programDto.endDate
        )

        return programRepository.save(program)
    }

    fun checkDate(email: String, startDate: LocalDate, endDate: LocalDate): Boolean {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val classId = teachesRepository.findById(user.id!!)
                .orElseThrow()
                .cId

        return !(programRepository.existsByClassIdAndStartDateLessThanEqualAndEndDateGreaterThanEqual(classId, startDate, startDate)
                || programRepository.existsByClassIdAndStartDateLessThanEqualAndEndDateGreaterThanEqual(classId, endDate, endDate))
    }

    fun getALlPrograms(email: String): List<Program> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val classId = if (user.role == "STUDENT") {
            belongsRepository.findById(user.id!!)
                .orElseThrow()
                .classId
        } else {
            teachesRepository.findById(user.id!!)
                .orElseThrow()
                .cId
        }

        return programRepository.findByClassIdOrderByStartDateAsc(classId)
    }

    fun getProgramsAfter(email: String, date: LocalDate): List<Program> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val classId = if (user.role == "STUDENT") {
            belongsRepository.findById(user.id!!)
                .orElseThrow()
                .classId
        } else {
            teachesRepository.findById(user.id!!)
                .orElseThrow()
                .cId
        }

        return programRepository.findByClassIdAndStartDateAfterOrderByStartDateAsc(classId, date)
    }

    fun getMyPrograms(email: String): List<Program> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        return joinsRepository.findByJoinsIdUserIdOrderByJoinsIdProgramStartDateAsc(user.id!!).map { joins ->
            joins.joinsId.program
        }
    }

    fun getCurrentProgram(email: String): Program? {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val program = if (user.role == "STUDENT") {
            programRepository.findCurrentProgramByUserId(user.id!!)
        } else {
            val cId = teachesRepository.findById(user.id!!)
                .orElseThrow()
                .cId

            programRepository.findCurrentProgramByClassId(cId)
        }

        return program
    }

    fun joinProgram(email: String, joinDto: JoinDto): Program {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val belongs = belongsRepository.findById(user.id!!)
            .orElseThrow()

        val program = programRepository.findByClassIdAndProgramName(belongs.classId, joinDto.programName)
            ?: throw Exception()

        joinsRepository.save(Joins(
            JoinsId(
                user = user,
                program = program
            )
        ))

        return program
    }

    fun cancelProgram(email: String, programName: String) {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val belongs = belongsRepository.findById(user.id!!)
            .orElseThrow()

        val program = programRepository.findByClassIdAndProgramName(belongs.classId, programName)
            ?: throw Exception("Program not found")

        joinsRepository.deleteById(JoinsId(user, program))
    }

    fun getClassStudents(email: String): List<User> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = teachesRepository.findById(user.id!!)
            .orElseThrow()
            .cId

        return belongsRepository.findByClassId(cId).map {
            it.user
        }
    }

    fun getProgramStudents(email: String, programName: String, sorted: Boolean?): List<User> {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val cId = teachesRepository.findById(user.id!!)
            .orElseThrow()
            .cId

        return if (sorted == true) {
            val program = programRepository.findByClassIdAndProgramName(cId, programName)
                ?: throw Exception("Program not found")

            joinsRepository.findProgramStudentsSorted(program.programId!!, program.startDate, program.endDate).map {
                User(
                    it.id,
                    it.email,
                    it.password,
                    it.name,
                    it.role
                )
            }
        } else {
            joinsRepository.findByJoinsIdProgramClassIdAndJoinsIdProgramProgramNameOrderByJoinsIdUserNameAsc(cId, programName).map {
                it.joinsId.user
            }
        }
    }

    fun getProgramStudentsCount(email: String, programName: String): Int {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val classId = if (user.role == "STUDENT") {
            belongsRepository.findById(user.id!!)
                .orElseThrow()
                .classId
        } else {
            teachesRepository.findById(user.id!!)
                .orElseThrow()
                .cId
        }

        val program = programRepository.findByClassIdAndProgramName(classId, programName)
            ?: throw Exception("Program not found")

        return joinsRepository.countByJoinsIdProgramProgramId(program.programId!!)
    }

    fun getProgramClassCount(email: String, date: LocalDate): Int {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val classId = if (user.role == "STUDENT") {
            belongsRepository.findById(user.id!!)
                .orElseThrow()
                .classId
        } else {
            teachesRepository.findById(user.id!!)
                .orElseThrow()
                .cId
        }

        val school = withinRepository.findById(classId)
            .orElseThrow()

        return programRepository.countProceedingClass(school.sId, date)
    }

    fun getSchoolCode(email: String): String {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val works = worksRepository.findByIdAndIsManagerTrue(user.id!!)
            ?: throw Exception("No school that ${user.name} manages exists")
        return works.sId
    }

    fun getClassCode(email: String): String {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        return teachesRepository.findById(user.id!!)
            .orElseThrow()
            .cId
    }

    fun getClassInfo(email: String): String {
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

        return classRepository.findById(cId)
            .orElseThrow()
            .className
    }

    fun getUserInfo(email: String): User {
        return userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")
    }

    fun saveVoice(email: String, voiceDto: VoiceDto) {
        val user = userRepository.findByEmail(email)
            ?: throw UsernameNotFoundException("User not found")

        val path = "./voice_files/${user.id!!}.m4a"
        val byteArray = Base64.getDecoder().decode(voiceDto.data)
        val file = File(path)
        if (file.exists()) {
            file.delete()
        }
        file.createNewFile()
        file.writeBytes(byteArray)
    }
}