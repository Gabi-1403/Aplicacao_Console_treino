Public Class Form1

    Public valorCredito As Double = 0
    Public tipoCredito As String
    Public parcelas As Integer = 0
    Public dataVencimento As Date = Date.MinValue
    Public mensagem As String = ""
    Public valorJurosMes As Double = 0
    Public valorJurosAno As Double = 0
    Public valorTotalApagar As Double = 0
    Public valorTotalJuros As Double = 0
    Public valorTotalAJuros As Double = 0

    Private Sub CheckedListBox1_SelectedIndexChanged(sender As Object, e As EventArgs)
        mensagem = ""
    End Sub

    Private Sub valorCriterio_TextChanged(sender As Object, e As EventArgs) Handles valorCriterio.TextChanged
        Try
            valorCredito = Double.Parse(valorCriterio.Text)
        Catch ex As Exception
            mensagem = "Recusado: Valor do crédito preenchido incorretamente."
        End Try
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Try
            parcelas = Integer.Parse(TextBox1.Text)
        Catch ex As Exception
            mensagem = "Recusado: Parcela preenchida incorretamente"
        End Try

    End Sub

    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles primeiraparcela.ValueChanged

        dataVencimento = primeiraparcela.Value

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim ret As Object = mensagem

        validacao()

        If mensagem = "" Then
            validacaoJuros()
        End If

        TextBox2_TextChanged(ret, e)
    End Sub
    Public Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

        If mensagem <> "" Then
            TextBox2.Text = mensagem
        Else
            TextBox2.Text = "Aprovado: Valor Total Juros: R$ " & Double.Parse(valorTotalJuros).ToString("#,##0.00") & ", valor Total com Juros: R$ " & Double.Parse(valorTotalApagar).ToString("#,##0.00")
        End If

        mensagem = ""

    End Sub
    Private Sub validacao()
        Dim valorMaximo As Double = 1000000
        Dim valorMinimoJur As Double = 15000
        Dim parcMin As Integer = 5
        Dim parcMax As Integer = 72
        If mensagem = "" Then
            'Valor do crédito
            If valorCredito > 0 Then
                If valorCredito > valorMaximo Then
                    mensagem = "Recusado: O valor máximo a ser liberado para qualquer tipo de empréstimo é de R$ 1.000.000,00"
                End If
            Else
                mensagem = "Recusado: Valor do crédito não preenchido"
            End If

            'Tipo de Crédito
            If tipoCredito = "Jurídico" AndAlso valorCredito < valorMinimoJur AndAlso mensagem = "" Then
                mensagem = "Recusado: Para o crédito de pessoa jurídica, o valor mínimo a ser liberado é de R$ 15.000,00"
            End If

            'Parcelas
            If parcelas < parcMin AndAlso mensagem = "" Then
                mensagem = "Recusado: A quantidade de parcelas mínima é de 5x"
            ElseIf parcelas > parcMax AndAlso mensagem = "" Then
                mensagem = "Recusado: A quantidade de parcelas máximas é de 72x"
            End If

            'Data de vencimento:
            Dim qtdDias As Integer = CInt(DateDiff(DateInterval.Day, Now.Date, dataVencimento.Date))

            If qtdDias < 15 AndAlso mensagem = "" Then
                Dim data As Date = DateAdd(DateInterval.Day, 15, Now.Date)
                mensagem = "Recusado: A data do primeiro vencimento deve ser a partir do dia " & data.ToString("dd/MM/yyyy")
            ElseIf qtdDias > 40 AndAlso mensagem = "" Then
                Dim data As Date = DateAdd(DateInterval.Day, 40, Now.Date)
                mensagem = "Recusado: A data do primeiro vencimento deve ser até o dia " & data.ToString("dd/MM/yyyy")
            End If
        End If
    End Sub

    Private Sub validacaoJuros()
        Dim CreditoDireto As Integer = 2 '- Taxa de 2% ao mês
        Dim CreditoConsignado As Integer = 1 '- Taxa de 1% ao mês
        Dim CreditoPessoaJuridica As Integer = 5 '- Taxa de 5% ao mês
        Dim CreditoPessoaFisica As Integer = 3 '- Taxa de 3% ao mês
        Dim CreditoImobiliário As Integer = 9 '- Taxa de 9% ao ano
        Dim paarc As New List(Of Integer)
        Dim parc As Integer = 0
        Dim cont As Integer = 0
        Dim temvalor As Double = False

        valorJurosMes = 0
        valorJurosAno = 0
        valorTotalApagar = 0
        valorTotalJuros = 0
        valorTotalAJuros = 0

        If mensagem = "" Then
            valorJurosMes += (valorCredito * CreditoDireto) / 100
            valorJurosMes += (valorCredito * CreditoConsignado) / 100
            If tipoCredito = "Jurídico" Then
                valorJurosMes += (valorCredito * CreditoPessoaJuridica) / 100
            Else
                valorJurosMes += (valorCredito * CreditoPessoaFisica) / 100
            End If

            valorJurosAno = (valorCredito * CreditoImobiliário) / 100


            'Calculo do valor total
            valorTotalApagar = valorCredito + (valorJurosMes * parcelas)

            While parc < parcelas
                parc += 1
                paarc.Add(parc)
            End While

            For i As Integer = 0 To paarc.Count - 1
                cont += 1
                If cont = 12 Then
                    cont = 0
                    valorTotalApagar += valorJurosAno
                    temvalor = True
                    valorTotalAJuros += valorJurosAno
                End If
            Next

            If Not temvalor Then
                valorTotalApagar += valorJurosAno
                valorTotalAJuros += valorJurosAno
            End If

            valorTotalJuros = (valorJurosMes * parcelas)
            valorTotalJuros += valorTotalAJuros
        End If

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            tipoCredito = ComboBox1.SelectedItem

        Catch ex As Exception
            mensagem = "Recusado: Preencher o tipo de crédito."
        End Try
    End Sub
End Class
