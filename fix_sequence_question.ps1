# PowerShell script to fix the sequence question specifically
Write-Host "🔧 Fix Sequence Question Tool" -ForegroundColor Green
Write-Host "=============================" -ForegroundColor Green

# Replace with your actual quiz ID
$quizId = 44  # Change this to the actual quiz ID

Write-Host "Fixing Quiz ID: $quizId" -ForegroundColor Yellow

try {
    # Step 1: Check current state
    Write-Host "`n📋 Step 1: Checking current state..." -ForegroundColor Cyan
    $quizDetails = Invoke-RestMethod -Uri "http://localhost:5000/api/quizzes/$quizId/detail" -Method GET
    
    $sequenceQuestion = $null
    foreach ($question in $quizDetails.questions) {
        if ($question.text -like "*sequence*" -or $question.text -like "*2, 4, 6, 8*") {
            $sequenceQuestion = $question
            break
        }
    }
    
    if ($sequenceQuestion) {
        Write-Host "Found sequence question!" -ForegroundColor Green
        Write-Host "Question: $($sequenceQuestion.text)" -ForegroundColor White
        Write-Host "Current CorrectAnswerIndex: $($sequenceQuestion.correctAnswerIndex)" -ForegroundColor Yellow
        
        # Check if it needs fixing
        if ($sequenceQuestion.correctAnswerIndex -eq 1) {
            Write-Host "✅ Question is already correct!" -ForegroundColor Green
            return
        }
        
        Write-Host "❌ Question needs fixing!" -ForegroundColor Red
    } else {
        Write-Host "❌ Sequence question not found!" -ForegroundColor Red
        return
    }
    
    # Step 2: Run the fix
    Write-Host "`n🔧 Step 2: Running the fix..." -ForegroundColor Cyan
    $fixResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/quizzes/$quizId/fix-answer-indices" -Method POST
    Write-Host "Fix response: $($fixResponse.message)" -ForegroundColor White
    
    # Step 3: Check the result
    Write-Host "`n✅ Step 3: Checking the result..." -ForegroundColor Cyan
    $updatedQuiz = Invoke-RestMethod -Uri "http://localhost:5000/api/quizzes/$quizId/detail" -Method GET
    
    $updatedSequenceQuestion = $null
    foreach ($question in $updatedQuiz.questions) {
        if ($question.text -like "*sequence*" -or $question.text -like "*2, 4, 6, 8*") {
            $updatedSequenceQuestion = $question
            break
        }
    }
    
    if ($updatedSequenceQuestion) {
        Write-Host "Updated sequence question:" -ForegroundColor Green
        Write-Host "Question: $($updatedSequenceQuestion.text)" -ForegroundColor White
        Write-Host "New CorrectAnswerIndex: $($updatedSequenceQuestion.correctAnswerIndex)" -ForegroundColor Green
        
        Write-Host "`nOptions:" -ForegroundColor Cyan
        for ($i = 0; $i -lt $updatedSequenceQuestion.options.Count; $i++) {
            $marker = if ($i -eq $updatedSequenceQuestion.correctAnswerIndex) { "✅ CORRECT" } else { "  " }
            Write-Host "  [$i] $($updatedSequenceQuestion.options[$i]) $marker" -ForegroundColor White
        }
        
        if ($updatedSequenceQuestion.correctAnswerIndex -eq 1) {
            Write-Host "`n🎉 SUCCESS! The sequence question is now fixed!" -ForegroundColor Green
            Write-Host "Answer '10' (index 1) will now be marked as CORRECT!" -ForegroundColor Green
        } else {
            Write-Host "`n⚠️ The fix didn't work as expected." -ForegroundColor Yellow
            Write-Host "You may need to manually update the database." -ForegroundColor Yellow
        }
    }
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure the API is running and the quiz ID is correct." -ForegroundColor Yellow
}

Write-Host "`n🎯 Next steps:" -ForegroundColor Green
Write-Host "1. Take the quiz again" -ForegroundColor White
Write-Host "2. Answer the sequence question with '10'" -ForegroundColor White
Write-Host "3. It should now show as CORRECT!" -ForegroundColor Green 