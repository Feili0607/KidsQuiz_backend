# PowerShell script to fix quiz answer indices
# This script demonstrates how to fix incorrect CorrectAnswerIndex values

Write-Host "🔧 Quiz Answer Index Fix Tool" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

# Replace with your actual quiz ID that has the sequence question
$quizId = 44  # Change this to the actual quiz ID

Write-Host "Attempting to fix answer indices for Quiz ID: $quizId" -ForegroundColor Yellow

try {
    # Call the fix endpoint
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/quizzes/$quizId/fix-answer-indices" -Method POST
    
    Write-Host "✅ Success!" -ForegroundColor Green
    Write-Host "Response: $($response.message)" -ForegroundColor White
    
    # Now let's check the quiz details to see the fixed answers
    Write-Host "`n📋 Checking quiz details..." -ForegroundColor Cyan
    $quizDetails = Invoke-RestMethod -Uri "http://localhost:5000/api/quizzes/$quizId/detail" -Method GET
    
    Write-Host "Quiz: $($quizDetails.title)" -ForegroundColor White
    Write-Host "Questions:" -ForegroundColor White
    
    foreach ($question in $quizDetails.questions) {
        Write-Host "  Question: $($question.text)" -ForegroundColor Yellow
        Write-Host "  Options:" -ForegroundColor Cyan
        for ($i = 0; $i -lt $question.options.Count; $i++) {
            $marker = if ($i -eq $question.correctAnswerIndex) { "✅" } else { "  " }
            Write-Host "    $marker [$i] $($question.options[$i])" -ForegroundColor White
        }
        Write-Host "  Correct Answer Index: $($question.correctAnswerIndex)" -ForegroundColor Green
        Write-Host "  Explanation: $($question.explanation)" -ForegroundColor Gray
        Write-Host ""
    }
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure the API is running and the quiz ID is correct." -ForegroundColor Yellow
}

Write-Host "`n🎯 To test the fix:" -ForegroundColor Green
Write-Host "1. Take the quiz again" -ForegroundColor White
Write-Host "2. Answer the sequence question with '10'" -ForegroundColor White
Write-Host "3. It should now show as CORRECT!" -ForegroundColor Green 