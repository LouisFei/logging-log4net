Appender:日志输出目的地，负责日志的输出（输出到什么地方）。

Appender控制日志的输出的目的地，一个输出源就叫一个Appender。
appender的类别有：Console（控制台），File(文件),JDBC，JMS等。

logger可以通过方法logger.addAppender(appender)，配置多个appender。
对logger来说，每个有效的日志请求结果都将输出到logger本身以及父logger的appender上。

常用的appender有：
ConsoleAppender（控制台）
FileAppender（文件）
DailyRollingFileAppender（每天产生一个日志文件）
WriterAppender(将日志信息以流格式发送到任意指定的地方)

