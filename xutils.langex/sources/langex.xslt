﻿<?xml version="1.0" encoding="utf-8"?>
<stylesheet
	version="2.0"
	xmlns="http://www.w3.org/1999/XSL/Transform"
	xmlns:ln="urn:langex"
	xmlns:ms="urn:schemas-microsoft-com:xslt"
	xmlns:ex="http://exslt.org/common"
	xmlns:fn="urn:user-functions"
	exclude-result-prefixes="ms fn ex">

	<output method="text" indent="no"/>
	<strip-space elements="*"/>
	
	<include href="utils.xslt" />
	
	<template match="/ln:unit">
		<text>//------------------------------------------------------------------------------&#xD;&#xA;</text>
		<text>// &lt;auto-generated&gt;                                                       &#xD;&#xA;</text>
		<text>//     This code was generated by a tool.                                       &#xD;&#xA;</text>
		<text>//                                                                              &#xD;&#xA;</text>
		<text>//     Changes to this file may cause incorrect behavior and will be lost if    &#xD;&#xA;</text>
		<text>//     the code is regenerated.                                                 &#xD;&#xA;</text>
		<text>// &lt;/auto-generated&gt;                                                      &#xD;&#xA;</text>
		<text>//------------------------------------------------------------------------------&#xD;&#xA;</text>
		<text>#pragma warning disable 1591&#xD;&#xA;</text>
		<apply-templates select="*" />
	</template>

	<template match="ln:use">
		<value-of select="fn:xprintfn('using {0};' , @name)" />
	</template>
	
	<template match="ln:namespace">
		<for-each select="use">
			<value-of select="fn:xprintfn('using {0};' , @clr-ns)" />
		</for-each>
		<value-of select="fn:xbeginf('namespace {0} {{' , @name)" />
		<apply-templates select="*"/>
		<value-of select="fn:xend('}')"/>
	</template>
	<template match="*" mode="append-generics">
		<if test="ln:generic">
			<text>&lt;</text>
			<for-each select="ln:generic">
				<value-of select="@name"/>
				<if test="not(position()=last())">
					<text>, </text>
				</if>
			</for-each>
			<text>&gt;</text>
		</if>
	</template>
	<template match="ln:class" >
		<value-of select="fn:xprint()" />
		<if test="@mods">
			<value-of select="fn:print(@mods, ' ')" />
		</if>
		<value-of select="fn:print('class ', @name)" />
		<apply-templates select="." mode="append-generics"/>
		
		<if test="ln:base">
			<text>: </text>
			<for-each select="ln:base">
				<value-of select="@name" />
				<apply-templates select="." mode="append-generics"/>
			</for-each>
		</if>
		
		<value-of select="fn:begin(' {')" />
		<apply-templates select="*"/>
		<value-of select="fn:xend('}')"/>
	</template>

	<template match="ln:interface" >
		<value-of select="fn:xprint()" />
		<if test=" @mods">
			<value-of select="fn:print(@mods, ' ')" />
		</if>
		<value-of select="fn:print('interface ', @name)" />
		<apply-templates select="." mode="append-generics"/>
		<if test="@base">
			<value-of select="fn:print(': ', @base)" />
		</if>
		<value-of select="fn:begin(' {')" />
		<apply-templates select="*"/>
		<value-of select="fn:xend('}')"/>
	</template>

	<template match="ln:enum" >
		<value-of select="fn:xprint()" />
		<if test=" @mods">
			<value-of select="fn:print(@mods, ' ')" />
		</if>
		<value-of select="fn:beginf('enum {0} {{',@name)" />
		<for-each select="ln:token">
			<value-of select="fn:xprint(@name)" />
			<if test="not(position()=last())">
				<text>,</text>
			</if>
			<value-of select="$CRLF"/>
		</for-each>
		<value-of select="fn:xend('}')"/>
	</template>

	<template match="ln:field" >
		<value-of select="fn:xprint()" />
		<if test="@mods">
			<value-of select="fn:print(@mods, ' ')" />
		</if>
		<value-of select="fn:printfn('{0} {1};', @type, @name)" />
	</template>
	
	<template match="ln:method | ln:delegate" mode="print-method-signature">
		<choose>
			<when test="@type">
				<value-of select="fn:print(@type, ' ')" />
			</when>
			<otherwise>
				<value-of select="fn:print('void ')" />
			</otherwise>
		</choose>
		<value-of select="fn:print(@name)" />
		<apply-templates select="." mode="append-generics"/>
		<value-of select="fn:print('(')" />
		<for-each select="ln:arg">
			<if test="ln:base">
				<value-of select="fn:print(ln:base/@name)" />
				<apply-templates select="ln:base" mode="append-generics" />
				<text>.</text>
			</if>
			<value-of select="fn:printf('{0} {1}', @type, @name)" />
			<if test="not(position()=last())">
				<text>, </text>
			</if>
		</for-each>
		<value-of select="fn:print(')')" />
	</template>

	<template match="ln:method" >
		<value-of select="fn:xprint()" />
		<if test="@mods">
			<value-of select="fn:print(@mods, ' ')" />
		</if>
		<apply-templates select="." mode="print-method-signature" />
		<choose>
			<when test="ln:emit">
				<value-of select="fn:begin(' {')" />
				<for-each select="ln:emit">
					<if test="@expr">
						<value-of select="fn:xprintfn('{0};', @expr)" />	
					</if>
				</for-each>
				<value-of select="fn:xend('}')" />
			</when>
			<otherwise>
				<value-of select="fn:printn(';')" />
			</otherwise>
		</choose>
	</template>

	<template match="ln:ctor" >
		<value-of select="fn:xprint()" />
		<if test="@mods">
			<value-of select="fn:print(@mods, ' ')" />
		</if>
		<value-of select="fn:print(../@name, '(')" />
		<for-each select="ln:arg">
			<value-of select="fn:printf('{0} {1}', @type, @name)" />
			<if test="not(position()=last())">
				<text>, </text>
			</if>
		</for-each>
		<value-of select="fn:print(')')" />
		<if test="ln:base">
			<text>: base(</text>
			<for-each select="ln:base">
				<value-of select="@expr" />
				<if test="not(position()=last())">
					<text>, </text>
				</if>
			</for-each>
			<text>)</text>
		</if>

		<value-of select="fn:begin(' {')" />
		<for-each select="ln:emit">
			<if test="@expr">
				<value-of select="fn:xprintfn('{0};', @expr)" />
			</if>
		</for-each>
		<value-of select="fn:xend('}')" />

	</template>


	<template match="ln:delegate" >
		<value-of select="fn:xprint()" />
		<if test="@mods">
			<value-of select="fn:print(@mods, ' ')" />
		</if>
		<text>delegate </text>
		<apply-templates select="." mode="print-method-signature" />
		<value-of select="fn:printn(';')" />
	</template>
	
	<template match="@* | node()"/>

</stylesheet>




