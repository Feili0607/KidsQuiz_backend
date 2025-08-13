# Manual fix script for the sequence question
Write-Host "🔧 Manual Fix Tool" -ForegroundColor Green
Write-Host "==================" -ForegroundColor Green

# Replace with your actual quiz ID
$quizId = 44  # Change this to the actual quiz ID

Write-Host "Manually fixing Quiz ID: $quizId" -ForegroundColor Yellow

try {
    # First, let's see what we're working with
    Write-Host "`n📋 Current quiz state:" -ForegroundColor Cyan
    $quizDetails = Invoke-RestMethod -Uri "http://localhost:5000/api/quizzes/$quizId/detail" -Method GET
    
    $sequenceQuestion = $null
    $questionIndex = -1
    
    for ($i = 0; $i -lt $quizDetails.questions.Count; $i++) {
        $question = $quizDetails.questions[$i]
        if ($question.text -like "*sequence*" -or $question.text -like "*2, 4, 6, 8*") {
            $sequenceQuestion = $question
            $questionIndex = $i
            break
        }
    }
    
    if (-not $sequenceQuestion) {
        Write-Host "❌ Sequence question not found!" -ForegroundColor Red
        return
    }
    
    Write-Host "Found sequence question at index $questionIndex" -ForegroundColor Green
    Write-Host "Question: $($sequenceQuestion.text)" -ForegroundColor White
    Write-Host "Current CorrectAnswerIndex: $($sequenceQuestion.correctAnswerIndex)" -ForegroundColor Yellow
    
    # Show current options
    Write-Host "`nCurrent options:" -ForegroundColor Cyan
    for ($i = 0; $i -lt $sequenceQuestion.options.Count; $i++) {
        $marker = if ($i -eq $sequenceQuestion.correctAnswerIndex) { "✅ CORRECT" } else { "  " }
        Write-Host "  [$i] $($sequenceQuestion.options[$i]) $marker" -ForegroundColor White
    }
    
    # Check if it's already correct
    if ($sequenceQuestion.correctAnswerIndex -eq 1) {
        Write-Host "`n✅ Question is already correct!" -ForegroundColor Green
        return
    }
    
    Write-Host "`n🚨 Question needs fixing!" -ForegroundColor Red
    Write-Host "Expected: CorrectAnswerIndex = 1 (for answer '10')" -ForegroundColor Red
    Write-Host "Current: CorrectAnswerIndex = $($sequenceQuestion.correctAnswerIndex)" -ForegroundColor Red
    
    # Try to fix it by updating the quiz
    Write-Host "`n🔧 Attempting to fix..." -ForegroundColor Cyan
    
    # Create a quiz update with the correct answer index
    $updateData = @{
        title = $quizDetails.title
        description = $quizDetails.description
        content = $quizDetails.content
        difficultyLevel = $quizDetails.difficultyLevel
        labels = $quizDetails.labels
    }
    
    $updateJson = $updateData | ConvertTo-Json
    
    # Update the quiz (this might trigger a refresh)
    $updateResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/quizzes/$quizId" -Method PUT -Body $updateJson -ContentType "application/json"
    Write-Host "Quiz update response: $($updateResponse.title)" -ForegroundColor White
    
    # Now try the automatic fix again
    Write-Host "`n🔄 Running automatic fix..." -ForegroundColor Cyan
    $fixResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/quizzes/$quizId/fix-answer-indices" -Method POST
    Write-Host "Fix response: $($fixResponse.message)" -ForegroundColor White
    
    # Check the final result
    Write-Host "`n✅ Checking final result..." -ForegroundColor Cyan
    $finalQuiz = Invoke-RestMethod -Uri "http://localhost:5000/api/quizzes/$quizId/detail" -Method GET
    
    $finalSequenceQuestion = $null
    foreach ($question in $finalQuiz.questions) {
        if ($question.text -like "*sequence*" -or $question.text -like "*2, 4, 6, 8*") {
            $finalSequenceQuestion = $question
            break
        }
    }
    
    if ($finalSequenceQuestion) {
        Write-Host "Final sequence question:" -ForegroundColor Green
        Write-Host "CorrectAnswerIndex: $($finalSequenceQuestion.correctAnswerIndex)" -ForegroundColor Green
        
        Write-Host "`nFinal options:" -ForegroundColor Cyan
        for ($i = 0; $i -lt $finalSequenceQuestion.options.Count; $i++) {
            $marker = if ($i -eq $finalSequenceQuestion.correctAnswerIndex) { "✅ CORRECT" } else { "  " }
            Write-Host "  [$i] $($finalSequenceQuestion.options[$i]) $marker" -ForegroundColor White
        }
        
        if ($finalSequenceQuestion.correctAnswerIndex -eq 1) {
            Write-Host "`n🎉 SUCCESS! The sequence question is now fixed!" -ForegroundColor Green
        } else {
            Write-Host "`n⚠️ The automatic fix didn't work." -ForegroundColor Yellow
            Write-Host "You may need to manually update the database or regenerate the quiz." -ForegroundColor Yellow
        }
    }
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure the API is running and the quiz ID is correct." -ForegroundColor Yellow
} 