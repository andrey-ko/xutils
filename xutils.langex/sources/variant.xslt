<?xml version="1.0" encoding="utf-8"?>
<stylesheet
	version="2.0"
	xmlns="http://www.w3.org/1999/XSL/Transform"
	xmlns:ln="urn:langex"
	xmlns:ms="urn:schemas-microsoft-com:xslt"
	xmlns:ex="http://exslt.org/common"
	xmlns:fn="urn:user-functions"
	exclude-result-prefixes="ms fn ex">

	<output method="xml" indent="yes"/>
	<strip-space elements="*"/>

	<include href="utils.xslt" />
	
	<template match="/">
		<choose>
			<when test="not(ln:unit)">
				<ln:unit>
					<apply-templates select="node() | @*"/>
				</ln:unit>
			</when>
			<otherwise>
				<copy>
					<apply-templates select="node() | @*"/>
				</copy>
			</otherwise>
		</choose>
	</template>
	

	<template match="ln:variant">
		<ln:class name="{@name}" mods="abstract {@mods}">

			<!-- Id enum -->
			<ln:enum name="Id" mods="public">
				<for-each select="ln:option | ln:group">
					<ln:token name="{fn:camel(@name)}"/>
				</for-each>
			</ln:enum>

			<!-- void imatch interface-->
			<ln:interface name="IMatch" mods="public">
				<for-each select="ln:option | ln:group">
					<ln:method name="On{@name}">
						<ln:arg name="{fn:camel(@name)}" type="{@name}" />
					</ln:method>
				</for-each>
			</ln:interface>
			
			<!-- generic imatch interface-->
			<ln:interface name="IMatch" mods="public">
				<ln:generic name="TMatch"/>
				<for-each select="ln:option | ln:group">
					<ln:method name="On{@name}" type="TMatch">
						<ln:arg name="{fn:camel(@name)}" type="{@name}" />
					</ln:method>
				</for-each>
			</ln:interface>

			<!-- void match callbacks -->
			<for-each select="ln:option | ln:group">
				<ln:delegate name="On{@name}Callback" mods="public" >
					<ln:arg name="{fn:camel(@name)}" type="{@name}" />
				</ln:delegate>
			</for-each>

			<!-- generic match callbacks -->
			<for-each select="ln:option | ln:group">
				<ln:delegate name="On{@name}Callback" type="TMatch" mods="public" >
					<ln:generic name="TMatch" />
					<ln:arg name="{fn:camel(@name)}" type="{@name}" />
				</ln:delegate>
			</for-each>

			<ln:field name="id" type="Id" mods="public readonly"/>
			<apply-templates select="*"/>

			<!-- constructor -->
			<ln:ctor mods="protected">
				<for-each select="ln:field">
					<ln:arg name="{@name}" type="{@type}" />
					<ln:emit expr="this.{@name} = {@name}"/>
				</for-each>
				<ln:arg name="id" type="Id" />
			</ln:ctor>

			<!-- void interface match -->
			<ln:method name="Match" mods="public abstract">
				<ln:arg name="handler" type="IMatch" />
			</ln:method>

			<!-- generic interface match -->
			<ln:method name="Match" type="TMatch" mods="public abstract">
				<ln:generic name="TMatch" />
				<ln:arg name="handler" type="IMatch&lt;TMatch&gt;" />
			</ln:method>

			<!-- void match -->
			<ln:method name="Match" mods="public abstract">
				<for-each select="ln:option | ln:group">
					<ln:arg name="{fn:camel(@name)}" type="On{@name}Callback" />
				</for-each>
			</ln:method>
			
			<!-- generic match -->
			<ln:method name="Match" type="TMatch" mods="public abstract">
				<ln:generic name="TMatch" />
				<for-each select="ln:option | ln:group">
					<ln:arg name="{fn:camel(@name)}" type="On{@name}Callback&lt;TMatch&gt;" />
				</for-each>
			</ln:method>

			<!-- IsXxx declaration & default impl-->
			<for-each select="ln:option | ln:group">
				<ln:method name="Is{@name}" type="bool" mods="public virtual">
					<ln:emit expr="return false"/>
				</ln:method>
			</for-each>

			<!-- AsXxx declaration & default impl-->
			<for-each select="ln:option | ln:group">
				<ln:method name="As{@name}" type="{@name}" mods="public virtual">
					<ln:emit expr="return null"/>
				</ln:method>
			</for-each>

			<!-- OnXxx declaration & default impl-->
			<for-each select="ln:option | ln:group">
				<ln:method name="On{@name}" type="bool" mods="public virtual">
					<ln:arg name="callback" type="On{@name}Callback" />
					<ln:emit expr="return false"/>
				</ln:method>
			</for-each>
		</ln:class>
	</template>

	<template match="ln:group">
		<variable name="base">
			<value-of select="../@name"/>
			<apply-templates mode="append-generics" select=".."/>
		</variable>

		<ln:class name="{@name}" mods="public abstract partial">
			<ln:base name="{../@name}">
				<copy-of select="../ln:generic"/>
			</ln:base>
			
			<!-- Id enum -->
			<ln:enum name="Id" mods="public new">
				<for-each select="ln:option | ln:group">
					<ln:token name="{fn:camel(@name)}"/>
				</for-each>
			</ln:enum>

			<!-- void imatch interface-->
			<ln:interface name="IMatch" mods="public new">
				<for-each select="ln:option | ln:group">
					<ln:method name="On{@name}">
						<ln:arg name="{fn:camel(@name)}" type="{@name}" />
					</ln:method>
				</for-each>
			</ln:interface>

			<!-- generic imatch interface-->
			<ln:interface name="IMatch" mods="public new">
				<ln:generic name="TMatch"/>
				<for-each select="ln:option | ln:group">
					<ln:method name="On{@name}" type="TMatch">
						<ln:arg name="{fn:camel(@name)}" type="{@name}" />
					</ln:method>
				</for-each>
			</ln:interface>

			<!-- void match callbacks -->
			<for-each select="ln:option | ln:group">
				<ln:delegate name="On{@name}Callback" mods="public" >
					<ln:arg name="{fn:camel(@name)}" type="{@name}" />
				</ln:delegate>
			</for-each>

			<!-- generic match callbacks -->
			<for-each select="ln:option | ln:group">
				<ln:delegate name="On{@name}Callback" type="TMatch" mods="public" >
					<ln:generic name="TMatch" />
					<ln:arg name="{fn:camel(@name)}" type="{@name}" />
				</ln:delegate>
			</for-each>
			
			<apply-templates select="*"/>

			<!-- constructor -->
			<ln:ctor mods="protected">
				<apply-templates select="." mode="ctor-args" />
				<ln:arg name="id" type="Id" />
				<apply-templates select=".." mode="ctor-base" />
				<ln:base expr="{$base}.Id.{fn:camel(@name)}" />
				<for-each select="ln:field">
					<ln:emit expr="this.{@name} = {@name}"/>
				</for-each>
			</ln:ctor>
			
			<!-- void interface match -->
			<ln:method name="Match" mods="public abstract">
				<ln:arg name="handler" type="IMatch" />
			</ln:method>

			<!-- generic interface match -->
			<ln:method name="Match" type="TMatch" mods="public abstract">
				<ln:generic name="TMatch" />
				<ln:arg name="handler" type="IMatch&lt;TMatch&gt;" />
			</ln:method>

			<!-- void match -->
			<ln:method name="Match" mods="public abstract">
				<for-each select="ln:option | ln:group">
					<ln:arg name="{fn:camel(@name)}" type="On{@name}Callback" />
				</for-each>
			</ln:method>

			<!-- generic match -->
			<ln:method name="Match" type="TMatch" mods="public abstract">
				<ln:generic name="TMatch" />
				<for-each select="ln:option | ln:group">
					<ln:arg name="{fn:camel(@name)}" type="On{@name}Callback&lt;TMatch&gt;" />
				</for-each>
			</ln:method>

			<!-- IsXxx declaration & default impl-->
			<for-each select="ln:option | ln:group">
				<ln:method name="Is{@name}" type="bool" mods="public virtual">
					<ln:emit expr="return false"/>
				</ln:method>
			</for-each>

			<!-- AsXxx declaration & default impl-->
			<for-each select="ln:option | ln:group">
				<ln:method name="As{@name}" type="{@name}" mods="public virtual">
					<ln:emit expr="return null"/>
				</ln:method>
			</for-each>

			<!-- OnXxx declaration & default impl-->
			<for-each select="ln:option | ln:group">
				<ln:method name="On{@name}" type="bool" mods="public virtual">
					<ln:arg name="callback" type="On{@name}Callback" />
					<ln:emit expr="return false"/>
				</ln:method>
			</for-each>

			<!-- void interface match impl -->
			<ln:method name="Match" mods="public override" >
				<ln:arg name="handler" type="IMatch">
					<ln:base name="{../@name}">
						<copy-of select="../ln:generic"/>
					</ln:base>
				</ln:arg>
				<ln:emit expr="handler.On{@name}(this)"/>
			</ln:method>

			<!-- generic interface match impl -->
			<ln:method name="Match" type="TMatch" mods="public override">
				<ln:generic name="TMatch" />
				<ln:arg name="handler" type="IMatch&lt;TMatch&gt;">
					<ln:base name="{../@name}">
						<copy-of select="../ln:generic"/>
					</ln:base>
				</ln:arg>
				<ln:emit expr="return handler.On{@name}(this)"/>
			</ln:method>

			<!-- void match impl -->
			<ln:method name="Match" mods="public override">
				<for-each select="../ln:option | ../ln:group">
					<ln:arg name="{fn:camel(@name)}" type="On{@name}Callback" />
				</for-each>
				<ln:emit expr="{fn:camel(@name)}(this)"/>
			</ln:method>

			<!-- generic match impl-->
			<ln:method name="Match" type="TMatch" mods="public override">
				<ln:generic name="TMatch" />
				<for-each select="../ln:option | ../ln:group">
					<ln:arg name="{fn:camel(@name)}" type="On{@name}Callback&lt;TMatch&gt;" />
				</for-each>
				<ln:emit expr="return {fn:camel(@name)}(this)"/>
			</ln:method>

			<!-- IsXxx declaration impl-->
			<ln:method name="Is{@name}" type="bool" mods="public override">
				<ln:emit expr="return true"/>
			</ln:method>

			<!-- AsXxx declaration impl-->
			<ln:method name="As{@name}" type="{@name}" mods="public override">
				<ln:emit expr="return this"/>
			</ln:method>

			<!-- OnXxx declaration impl-->
			<ln:method name="On{@name}" type="bool" mods="public override">
				<ln:arg name="callback" type="On{@name}Callback" />
				<ln:emit expr="callback(this)"/>
				<ln:emit expr="return true"/>
			</ln:method>
		</ln:class>
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
	
	<template match="ln:option">
		<ln:class name="{@name}" mods="public">
			<ln:base name="{../@name}">
				<copy-of select="../ln:generic"/>
			</ln:base>
			
			<apply-templates select="*"/>

			<!-- constructor -->
			<ln:ctor mods="public">
				<apply-templates select="." mode="ctor-args" />
				<apply-templates select=".." mode="ctor-base" />
				<ln:base expr="Id.{fn:camel(@name)}" />
				<for-each select="ln:field">
					<ln:emit expr="this.{@name} = {@name}"/>
				</for-each>
			</ln:ctor>

			<!-- void interface match impl -->
			<ln:method name="Match" mods="public override" >
				<ln:arg name="handler" type="IMatch" />
				<ln:emit expr="handler.On{@name}(this)"/>
			</ln:method>

			<!-- generic interface match impl -->
			<ln:method name="Match" type="TMatch" mods="public override">
				<ln:generic name="TMatch" />
				<ln:arg name="handler" type="IMatch&lt;TMatch&gt;" />
				<ln:emit expr="return handler.On{@name}(this)"/>
			</ln:method>

			<!-- void match impl -->
			<ln:method name="Match" mods="public override">
				<for-each select="../ln:option | ../ln:group">
					<ln:arg name="{fn:camel(@name)}" type="On{@name}Callback" />
				</for-each>
				<ln:emit expr="{fn:camel(@name)}(this)"/>
			</ln:method>

			<!-- generic match impl-->
			<ln:method name="Match" type="TMatch" mods="public override">
				<ln:generic name="TMatch" />
				<for-each select="../ln:option | ../ln:group">
					<ln:arg name="{fn:camel(@name)}" type="On{@name}Callback&lt;TMatch&gt;" />
				</for-each>
				<ln:emit expr="return {fn:camel(@name)}(this)"/>
			</ln:method>

			<!-- IsXxx declaration impl-->
			<ln:method name="Is{@name}" type="bool" mods="public override">
				<ln:emit expr="return true"/>
			</ln:method>

			<!-- AsXxx declaration impl-->
			<ln:method name="As{@name}" type="{@name}" mods="public override">
				<ln:emit expr="return this"/>
			</ln:method>

			<!-- OnXxx declaration impl-->
			<ln:method name="On{@name}" type="bool" mods="public override">
				<ln:arg name="callback" type="On{@name}Callback" />
				<ln:emit expr="callback(this)"/>
				<ln:emit expr="return false"/>
			</ln:method>
		</ln:class>
	</template>

	<template match="ln:option | ln:group" mode="ctor-args">
		<for-each select="ln:field">
			<ln:arg name="{@name}" type="{@type}" />
		</for-each>
		<apply-templates select=".." mode="ctor-args" />
	</template>
	<template match="ln:variant" mode="ctor-args">
		<for-each select="ln:field">
			<ln:arg name="{@name}" type="{@type}" />	
		</for-each>
	</template>
	<template match="@* | node()" mode="ctor-args" />

	
	<template match="ln:option | ln:group" mode="ctor-base">
		<for-each select="ln:field">
			<ln:base expr="{@name}"/>
		</for-each>
		<apply-templates select=".." mode="ctor-base" />
	</template>
	<template match="ln:variant" mode="ctor-base">
		<for-each select="ln:field">
			<ln:base expr="{@name}"/>
		</for-each>
	</template>
	<template match="@* | node()" mode="ctor-base" />
		

	<template match="@* | node()">
		<copy>
			<apply-templates select="@* | node()"/>
		</copy>
	</template>

</stylesheet>




