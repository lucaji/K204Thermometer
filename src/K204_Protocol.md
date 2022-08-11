# VOLTCRAFT K204

## DIGITAL 4 CHANNEL THERMOMETER

### RANGES

-200°C - 1370°C
-328°F - 2498°F

### RS232 SERIAL PROTOCOL

This is a revision of the available PDF specifying the K204 protocol available online, which is not complete nor clear.

Serial setup: 9600-8N1

### COMMANDS

According the available documentation, only one command is supported (A).

## A

Sending the "A" command (0x40, letter 'A'), returns a 45 bytes packet using big endianness:

    example:
    02 80 06 01 02 02 02 00 EF 7F FF 7F FF 7F FF 00
    00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
    00 00 00 00 00 00 00 0E 00 00 00 0E 03


Byte 00:

    example: 0x02 = 0b00000010

    Header - must be 0x02

Byte 01:

    example: 0x80 = 0b10000000

    bit 0 = 0 not in REC mode(it can’t use in b/n100806)
    bit 2 & bit 1 = 00 normal mode(no max/min)
    bit 3 = 0 that means not in the type of T1-T2
    bit 4 = 0 not in REL mode
    bit 5 = 0 not in HOLD mode
    bit 6 = 0 battery is not low
    bit 8 = 1 C

Byte 02:

    example: 0x06 = 0b00000110

    bit 0= 0 Memory is not full (it can’t use in b/n100806)
    bit 1
    bit 2
    bit 3
    bit 4
    bit 5
    bit 6
    bit 7=1 1 in auto power off mode

Byte 03 - Byte 06:
    Not used.


In generally, from 8th to 15th bytes shall be parsed as below:

8th and 9th byte is the value of channel 1: 0x00EF, decimal is 239, divide by 10 is 23.9 (read below for resolution)
10th and 11th : byte is the value of channel 2.
12th and 13th : byte is the value of channel 3.
14th and 15th : byte is the value of channel 4.
In the type of REL, the byte from 16th to 23 th.
In the type of MIN, the byte from 24th to 31 th.
In the type of MAX, the byte from 32th to 39 th.

40th BYTE: In generally, per channel become OL, should be see the byte as below:

    example: 0x0E = 0b00001110

    bit 0= 0 channel 1 is not OL
    bit 1 =1 channel 2 is OL.
    bit 2 =1 channel 3 is OL.
    bit 3=1 channel 4 is OL.
    bit 4 NOT USED
    bit 5 NOT USED
    bit 6 NOT USED
    bit 7 NOT USED

41rd BYTE: In the type of REL, we need to see the byte if the channel show OL as below:

    example: 0x0E = 0b00001110

    bit 0= 0 channel 1 is not OL
    bit 1 =1 channel 2 is OL.
    bit 2 =1 channel 3 is OL.
    bit 3=1 channel 4 is OL.
    bit 4 NOT USED
    bit 5 NOT USED
    bit 6 NOT USED
    bit 7 NOT USED

42rd BYTE: In the type of MAX, we need to see the byte if the channel show OL as below:

    example: 0x0E = 0b00001110

    bit 0= 0 channel 1 is not OL
    bit 1 =1 channel 2 is OL.
    bit 2 =1 channel 3 is OL.
    bit 3=1 channel 4 is OL.
    bit 4 NOT USED
    bit 5 NOT USED
    bit 6 NOT USED
    bit 7 NOT USED

43rd BYTE: In the type of MIN, we need to see the byte if the channel show OL as below:

    example: 0x0E = 0b00001110

    bit 0= 0 channel 1 is not OL
    bit 1 =1 channel 2 is OL.
    bit 2 =1 channel 3 is OL.
    bit 3=1 channel 4 is OL.
    bit 4 NOT USED
    bit 5 NOT USED
    bit 6 NOT USED
    bit 7 NOT USED

44rd BYTE: the resolution of per channel as below:

    example: 0x0E = 0b00001110

    bit 0= 0 channel 1the figure out need to divide 10
    bit 1 =1 channel 2 the figure out doesn’t need to divide 10
    bit 2 =1 channel 3 the figure out doesn’t need to divide 10
    bit 3=1 channel 4 the figure out doesn’t need to divide 10
    bit 4 NOT USED
    bit 5 NOT USED
    bit 6 NOT USED
    bit 7 NOT USED

45rd BYTE should be 03




-- 

Revision 1
20211109