package com.noswear.noswear.controller

import com.noswear.noswear.dto.JoinDto
import com.noswear.noswear.dto.ProgramDto
import com.noswear.noswear.dto.UserResponse
import com.noswear.noswear.dto.VoiceDto
import com.noswear.noswear.service.ManageService
import org.springframework.http.ResponseEntity
import org.springframework.security.access.prepost.PreAuthorize
import org.springframework.security.core.context.SecurityContextHolder
import org.springframework.stereotype.Controller
import org.springframework.web.bind.annotation.DeleteMapping
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.PostMapping
import org.springframework.web.bind.annotation.RequestBody
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RequestParam
import java.time.LocalDate

@RequestMapping("/manage")
@Controller
class ManageController(
    private val manageService: ManageService
) {
    @PostMapping("/program/create")
    @PreAuthorize("hasAnyRole('MANAGER', 'TEACHER')")
    fun createProgram(@RequestBody programDto: ProgramDto): ResponseEntity<ProgramDto> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val program = manageService.createProgram(name, programDto)
        return ResponseEntity.ok(ProgramDto.of(program))
    }

    @GetMapping("/program/check-date")
    @PreAuthorize("hasRole('TEACHER')")
    fun checkDate(startDate: LocalDate, endDate: LocalDate): ResponseEntity<Boolean> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val result = manageService.checkDate(name, startDate, endDate)
        return ResponseEntity.ok(result)
    }

    @GetMapping("/program/get/all")
    @PreAuthorize("isAuthenticated()")
    fun getAllPrograms(): ResponseEntity<List<ProgramDto>> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val programs = mutableListOf<ProgramDto>()
        manageService.getALlPrograms(name).map { program ->
            programs.add(ProgramDto.of(program))
        }
        return ResponseEntity.ok(programs)
    }

    @GetMapping("/program/get/after")
    @PreAuthorize("isAuthenticated()")
    fun getProgramsAfter(date: LocalDate): ResponseEntity<List<ProgramDto>> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val programs = mutableListOf<ProgramDto>()
        manageService.getProgramsAfter(name, date).map { program ->
            programs.add(ProgramDto.of(program))
        }
        return ResponseEntity.ok(programs)
    }

    @GetMapping("/program/get/me")
    @PreAuthorize("hasRole('STUDENT')")
    fun getMyProgram(): ResponseEntity<List<ProgramDto>> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val programs = mutableListOf<ProgramDto>()
        manageService.getMyPrograms(name).map { program ->
            programs.add(ProgramDto.of(program))
        }
        return ResponseEntity.ok(programs)
    }

    @GetMapping("/program/get/current")
    @PreAuthorize("isAuthenticated()")
    fun getCurrentProgram(): ResponseEntity<ProgramDto?> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val program = manageService.getCurrentProgram(name)
        return ResponseEntity.ok(if (program != null) {
            ProgramDto.of(program)
        } else {
            null
        })
    }

    @PostMapping("/program/join")
    @PreAuthorize("hasRole('STUDENT')")
    fun joinProgram(@RequestBody joinDto: JoinDto): ResponseEntity<ProgramDto> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val program = manageService.joinProgram(name, joinDto)
        return ResponseEntity.ok(ProgramDto.of(program))
    }

    @DeleteMapping("/program/join")
    @PreAuthorize("hasRole('STUDENT')")
    fun cancelProgram(programName: String): ResponseEntity<String> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        manageService.cancelProgram(name, programName)
        return ResponseEntity.ok("success")
    }

    @GetMapping("/students/all")
    @PreAuthorize("hasAnyRole('MANAGER', 'TEACHER')")
    fun getClassStudents(): ResponseEntity<List<UserResponse>> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val result = manageService.getClassStudents(name)
        return ResponseEntity.ok(result.map { user ->
            UserResponse.of(user)
        })
    }

    @GetMapping("/students/program")
    @PreAuthorize("hasAnyRole('MANAGER', 'TEACHER')")
    fun getProgramStudents(programName: String, @RequestParam(required = false) sorted: Boolean?): ResponseEntity<List<UserResponse>> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val result = manageService.getProgramStudents(name, programName, sorted)
        return ResponseEntity.ok(result.map { user ->
            UserResponse.of(user)
        })
    }

    @GetMapping("/students/program/count")
    @PreAuthorize("isAuthenticated()")
    fun getProgramStudentsCount(programName: String): ResponseEntity<Int> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val result = manageService.getProgramStudentsCount(name, programName)
        return ResponseEntity.ok(result)
    }

    @GetMapping("/class/program/count")
    @PreAuthorize("isAuthenticated()")
    fun getProgramClassCount(date: LocalDate): ResponseEntity<Int> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val result = manageService.getProgramClassCount(name, date)
        return ResponseEntity.ok(result)
    }

    @GetMapping("/code/school")
    @PreAuthorize("hasAnyRole('MANAGER', 'TEACHER')")
    fun getSchoolCode(): ResponseEntity<String> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val schoolId = manageService.getSchoolCode(name)
        return ResponseEntity.ok(schoolId)
    }

    @GetMapping("/code/class")
    @PreAuthorize("hasAnyRole('MANAGER', 'TEACHER')")
    fun getClassCode(): ResponseEntity<String> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val classId = manageService.getClassCode(name)
        return ResponseEntity.ok(classId)
    }

    @GetMapping("/info/class")
    @PreAuthorize("isAuthenticated()")
    fun getClassInfo(): ResponseEntity<String> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val className = manageService.getClassInfo(name)
        return ResponseEntity.ok(className)
    }

    @GetMapping("/info/user")
    @PreAuthorize("isAuthenticated()")
    fun getUserInfo(): ResponseEntity<UserResponse> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        val user = manageService.getUserInfo(name)
        return ResponseEntity.ok(UserResponse.of(user))
    }

    @PostMapping("/voice")
    @PreAuthorize("hasRole('STUDENT')")
    fun saveVoice(@RequestBody voiceDto: VoiceDto): ResponseEntity<String> {
        val authentication = SecurityContextHolder.getContext().authentication
        val name = authentication.name

        manageService.saveVoice(name, voiceDto)
        return ResponseEntity.ok("success")
    }
}