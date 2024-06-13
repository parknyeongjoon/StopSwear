package com.noswear.noswear

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.runApplication

@SpringBootApplication
class NoSwearApplication

fun main(args: Array<String>) {
    runApplication<NoSwearApplication>(*args)
}
