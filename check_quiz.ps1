# PowerShell script to check quiz details and see the current state
Write-Host "🔍 Quiz Check Tool" -ForegroundColor Green
Write-Host "==================" -ForegroundColor Green

# Replace with your actual quiz ID
$quizId = 44  # Change this to the actual quiz ID

Write-Host "Checking Quiz ID: $quizId" -ForegroundColor Yellow

try {
    # Get quiz details
    $quizDetails = Invoke-RestMethod -Uri "http://localhost:5000/api/quizzes/$quizId/detail" -Method GET
    
    Write-Host "`n📋 Quiz Details:" -ForegroundColor Cyan
    Write-Host "Title: $($quizDetails.title)" -ForegroundColor White
    Write-Host "Description: $($quizDetails.description)" -ForegroundColor White
    
    Write-Host "`n❓ Questions:" -ForegroundColor Yellow
    foreach ($question in $quizDetails.questions) {
        Write-Host "`nQuestion: $($question.text)" -ForegroundColor White
        
        Write-Host "Options:" -ForegroundColor Cyan
        for ($i = 0; $i -lt $question.options.Count; $i++) {
            $marker = if ($i -eq $question.correctAnswerIndex) { "✅ CORRECT" } else { "  " }
            Write-Host "  [$i] $($question.options[$i]) $marker" -ForegroundColor White
        }
        
        Write-Host "Correct Answer Index: $($question.correctAnswerIndex)" -ForegroundColor Green
        Write-Host "Explanation: $($question.explanation)" -ForegroundColor Gray
        
        # Check if this is the sequence question
        if ($question.text -like "*sequence*" -or $question.text -like "*2, 4, 6, 8*") {
            Write-Host "`n🚨 SEQUENCE QUESTION DETECTED!" -ForegroundColor Red
            Write-Host "Current CorrectAnswerIndex: $($question.correctAnswerIndex)" -ForegroundColor Red
            Write-Host "Expected CorrectAnswerIndex: 1 (for answer '10')" -ForegroundColor Red
            
            if ($question.correctAnswerIndex -eq 1) {
                Write-Host "✅ This question is already correct!" -ForegroundColor Green
            } else {
                Write-Host "❌ This question needs fixing!" -ForegroundColor Red
            }
        }
    }
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure the API is running and the quiz ID is correct." -ForegroundColor Yellow
} 