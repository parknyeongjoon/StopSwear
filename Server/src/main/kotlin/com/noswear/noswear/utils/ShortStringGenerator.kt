package com.noswear.noswear.utils

import org.hibernate.engine.spi.SharedSessionContractImplementor
import org.hibernate.id.IdentifierGenerator
import java.io.Serializable
import java.math.BigInteger
import java.security.MessageDigest
import java.util.*

class ShortStringGenerator : IdentifierGenerator {
    override fun generate(session: SharedSessionContractImplementor?, `object`: Any?): Serializable {
        val uuid = UUID.randomUUID().toString()

        val md = MessageDigest.getInstance("MD5")
        val digest = md.digest(uuid.toByteArray())
        val bigInt = BigInteger(1, digest)
        val hash = bigInt.toString(16)
        val encoded = Base64.getUrlEncoder().encodeToString(hash.toByteArray())

        return encoded.substring(0, 10)
    }
}