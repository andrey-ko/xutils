<?xml version="1.0" encoding="utf-8"?>
<stylesheet
	version="2.0"
	xmlns="http://www.w3.org/1999/XSL/Transform"
	xmlns:ms="urn:schemas-microsoft-com:xslt"
	xmlns:ex="http://exslt.org/common"
	exclude-result-prefixes="ms"
	xmlns:fn="urn:user-functions"
	xmlns:ln="urn:ext-lang">

	<variable name="CR" select="'&#xD;'"/>
	<variable name="LF" select="'&#xA;'"/>
	<variable name="CRLF" select="'&#xD;&#xA;'"/>
	<variable name="TAB" select="'&#x9;'" />

	<ms:script implements-prefix="fn" language='CSharp'>
		<ms:assembly name="System.Core" />
		<ms:using namespace="System.Linq" />
		<![CDATA[
		int m_ident = 0;
		
		public string begin() {
			m_ident += 1;
			return "\n";
		}
		public string begin(string s1) {
			m_ident += 1;
			return string.Concat(s1, "\n");
		}
		public string beginf(string f, string s1) {
			m_ident += 1;
			return string.Concat(
				String.Format(f, s1), "\n"
			);
		}
		public string beginf(string f, string s1, string s2) {
			m_ident += 1;
			return string.Concat(
				String.Format(f, s1, s2), "\n"
			);
		}
		public string xbegin(string s1) {
			return string.Concat(ident(m_ident++), s1, "\n");
		}
		public string xbegin(string s1, string s2) {
			return string.Concat(ident(m_ident++), s1, s2, "\n");
		}
		public string xbeginf(string s1, string s2) {
			return string.Concat(ident(m_ident++), String.Format(s1, s2), "\n");
		}
		public string xbeginf(string f, string s1, string s2) {
			return string.Concat(
				ident(m_ident++), String.Format(f, s1, s2), "\n"
			);
		}
		public string xbeginf(string f, string s1, string s2, string s3) {
			return string.Concat(
				ident(m_ident++), String.Format(f, s1, s2, s3), "\n"
			);
		}
		public string xbeginf(string f, string s1, string s2, string s3, string s4) {
			return string.Concat(
				ident(m_ident++), String.Format(f, s1, s2, s3, s4), "\n"
			);
		}
		
		public string xend() {
			return string.Concat(
				ident(--m_ident), "\n"
			);
		}
		public string xend(string s1) {
			return string.Concat(
				ident(--m_ident), s1, "\n"
			);
		}
		public string xendf(string f, string s1) {
			return string.Concat(
				ident(--m_ident), String.Format(f, s1), "\n"
			);
		}
		
		public string xlabel(string s1) {
			return string.Concat(
				ident(m_ident-1), s1
			);
		}
		public string xlabel(string s1, string s2) {
			return string.Concat(
				ident(m_ident-1), s1, s2
			);
		}
		public string xlabeln(string s1) {
			return string.Concat(
				ident(m_ident-1), s1, "\n"
			);
		}
		public string xlabeln(string s1, string s2) {
			return string.Concat(
				ident(m_ident-1), s1, s2, "\n"
			);
		}
		public string xlabelf(string f, string s1) {
			return string.Concat(
				ident(m_ident-1), String.Format(f, s1)
			);
		}
		public string xlabelfn(string f, string s1) {
			return string.Concat(
				ident(m_ident-1), String.Format(f, s1), "\n"
			);
		}

		public string xprint() {
			return ident(m_ident);
		}
		public string xprint(string s1) {
			return string.Concat(
				ident(m_ident), s1
			);
		}
		public string xprint(string s1, string s2) {
			return string.Concat(
				ident(m_ident), s1, s2
			);
		}
		public string xprint(string s1, string s2, string s3) {
			return string.Concat(
				ident(m_ident), s1, s2, s3
			);
		}
		
		public string xprintn() {
			return string.Concat(
				ident(m_ident), "\n"
			);
		}
		public string xprintn(string s1) {
			return string.Concat(
				ident(m_ident), s1, "\n"
			);
		}
		public string xprintn(string s1, string s2) {
			return string.Concat(
				ident(m_ident), s1, s2, "\n"
			);
		}
		public string xprintn(string s1, string s2, string s3) {
			return string.Concat(
				ident(m_ident), s1, s2, s3, "\n"
			);
		}
		
		public string xprintf(string f, string s1) {
			return string.Concat(
				ident(m_ident), String.Format(f, s1)
			);
		}
		public string xprintf(string f, string s1, string s2) {
			return string.Concat(
				ident(m_ident), String.Format(f, s1, s2)
			);
		}
		public string xprintf(string f, string s1, string s2, string s3) {
			return string.Concat(
				ident(m_ident), String.Format(f, s1, s2, s3)
			);
		}
		
		public string xprintfn(string f, string s1) {
			return string.Concat(
				ident(m_ident), String.Format(f, s1), "\n"
			);
		}
		public string xprintfn(string f, string s1, string s2) {
			return string.Concat(
				ident(m_ident), String.Format(f, s1, s2), "\n"
			);
		}
		public string xprintfn(string f, string s1, string s2, string s3) {
			return string.Concat(
				ident(m_ident), String.Format(f, s1, s2, s3), "\n"
			);
		}
		
		public string camel(string str) {
			if(String.IsNullOrEmpty(str)){
				return str;
			}
			var sb = new StringBuilder(str);
			sb[0] = Char.ToLowerInvariant(str[0]);
			return sb.ToString();
		}
		public string repeat(string str, int count) {
			return string.Concat(Enumerable.Repeat(str, count));
		}
		public string ident(int count) {
			return new string('\t', count);
		}
		public string printf(string f, string s1) {
			return string.Format(f, s1);
		}
		public string printf(string f, string s1, string s2) {
			return string.Format(f, s1, s2);
		}
		public string printf(string f, string s1, string s2, string s3) {
			return string.Format(f, s1, s2, s3);
		}
		
		public string printfn(string f, string s1) {
			return string.Concat(
				string.Format(f, s1), "\n"
			);
		}
		public string printfn(string f, string s1, string s2) {
			return string.Concat(
				string.Format(f, s1, s2), "\n"
			);
		}
		public string printfn(string f, string s1, string s2, string s3) {
			return string.Concat(
				string.Format(f, s1, s2, s3), "\n"
			);
		}
		
		
		public string print(string s1) {
			return s1;
		}
		public string print(string s1, string s2) {
			return string.Concat(s1, s2);
		}
		public string print(string s1, string s2, string s3) {
			return string.Concat(s1, s2, s3);
		}
		public string print(string s1, string s2, string s3, string s4) {
			return string.Concat(s1, s2, s3, s4);
		}
		public string print(string s1, string s2, string s3, string s4, string s5) {
			return string.Concat(s1, s2, s3, s4, s5);
		}
		
		public string printn(string s1) {
			return string.Concat(s1, "\n");
		}
		public string printn(string s1, string s2) {
			return string.Concat(s1, s2, "\n");
		}
		public string printn(string s1, string s2, string s3) {
			return string.Concat(s1, s2, s3, "\n");
		}
		public string printn(string s1, string s2, string s3, string s4) {
			return string.Concat(s1, s2, s3, s4, "\n");
		}
		public string printn(string s1, string s2, string s3, string s4, string s5) {
			return string.Concat(s1, s2, s3, s4, s5, "\n");
		}
		]]>
	</ms:script>

</stylesheet>
