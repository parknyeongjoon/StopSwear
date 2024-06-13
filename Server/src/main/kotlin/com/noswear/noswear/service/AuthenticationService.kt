package com.noswear.noswear.service

import com.noswear.noswear.domain.*
import com.noswear.noswear.dto.LoginDto
import com.noswear.noswear.dto.StudentDto
import com.noswear.noswear.dto.TeacherDto
import com.noswear.noswear.repository.*
import org.springframework.security.authentication.AuthenticationManager
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken
import org.springframework.security.core.userdetails.UsernameNotFoundException
import org.springframework.security.crypto.password.PasswordEncoder
import org.springframework.stereotype.Service
import org.springframework.transaction.annotation.Transactional
import java.io.File
import java.util.*

@Service
class AuthenticationService(
    val userRepository: UserRepository,
    val schoolRepository: SchoolRepository,
    val worksRepository: WorksRepository,
    val classRepository: ClassRepository,
    val withinRepository: WithinRepository,
    val teachesRepository: TeachesRepository,
    val belongsRepository: BelongsRepository,
    val passwordEncoder: PasswordEncoder,
    val authenticationManager: AuthenticationManager
) {
    @Transactional
    fun teacherRegister(teacherDto: TeacherDto): User {
        val teacherEntity = User(
            email = teacherDto.email,
            password = passwordEncoder.encode(teacherDto.password),
            name = teacherDto.name,
            role = if (teacherDto.create) "MANAGER" else "TEACHER"
        )

        val teacherResult = userRepository.save(teacherEntity)

        lateinit var schoolId: String
        if (teacherDto.create) {
            val schoolEntity = School(
                schoolName = teacherDto.schoolName!!
            )

            val schoolResult = schoolRepository.save(schoolEntity)

            schoolId = schoolResult.schoolId!!
        } else {
            schoolId = teacherDto.schoolId!!
        }

        val worksEntity = Works(
            id = teacherResult.id!!,
            sId = schoolId,
            isManager = teacherDto.create
        )

        worksRepository.save(worksEntity)

        val classEntity = Class(
            className = teacherDto.className
        )

        val classResult = classRepository.save(classEntity)

        val withinEntity = Within(
            cId = classResult.classId!!,
            sId = schoolId
        )

        withinRepository.save(withinEntity)

        val teachesEntity = Teaches(
            id = teacherResult.id!!,
            cId = classResult.classId!!
        )

        teachesRepository.save(teachesEntity)

        return teacherResult
    }

    @Transactional
    fun studentRegister(studentDto: StudentDto): User {
        val studentEntity = User(
            email = studentDto.email,
            password = passwordEncoder.encode(studentDto.password),
            name = studentDto.name,
            role = "STUDENT"
        )

        val studentResult = userRepository.save(studentEntity)

        val belongsEntity = Belongs(
            user = studentResult,
            classId = studentDto.classId
        )

        belongsRepository.save(belongsEntity)

        val path = "./voice_files/${studentResult.id!!}.m4a"
        val byteArray = Base64.getDecoder().decode(studentDto.voice)
        val file = File(path)
        file.writeBytes(byteArray)

        return studentResult
    }

    fun authenticate(loginDto: LoginDto): User {
        authenticationManager.authenticate(
            UsernamePasswordAuthenticationToken(
                loginDto.email,
                loginDto.password
            )
        )

        return userRepository.findByEmail(loginDto.email)
            ?: throw UsernameNotFoundException("User not found")
    }

    fun emailExists(email: String): Boolean {
        return userRepository.existsByEmail(email)
    }

    fun schoolExists(schoolId: String): Boolean {
        return schoolRepository.existsBySchoolId(schoolId)
    }

    fun classExists(classId: String): Boolean {
        return classRepository.existsByClassId(classId)
    }
}