Imports System

Module Program
    Sub Main(args As String())
        
    End Sub
    
    Sub CrossJoinCities()
        Dim ws As Worksheet
        Set ws = ThisWorkbook.Sheets("Sheet1") ' Замените "Sheet1" на имя вашего листа

        Dim dataRange As Range
        Set dataRange = ws.Range("A2:D" & ws.Cells(Rows.Count, 1).End(xlUp).Row) ' Определяет диапазон данных

        Dim dict As Object
        Set dict = CreateObject("Scripting.Dictionary")

        Dim key As String
        Dim rowData As Range

        ' Заполнение словаря
        For Each rowData In dataRange.Rows
            key = rowData.Cells(2).Value & "_" & rowData.Cells(3).Value
            If Not dict.Exists(key) Then
                dict(key) = rowData.Cells(4).Value
            Else
                dict(key) = dict(key) & ", " & rowData.Cells(4).Value
            End If
        Next rowData

        ' Запись результата в новый столбец
        Dim outputRange As Range
        Set outputRange = ws.Range("F2")

        For Each key In dict.Keys
            outputRange.Value = key
            outputRange.Offset(0, 1).Value = dict(key)
            Set outputRange = outputRange.Offset(1, 0)
        Next key
    End Sub
    
End Module
