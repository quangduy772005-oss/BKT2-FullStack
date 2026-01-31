$body = @{
    email = "teacher@pcm.local"
    password = "Teacher@123"
    fullName = "Giao Vien Cham Bai"
    phoneNumber = "0123456789"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5140/api/auth/register" -Method POST -ContentType "application/json" -Body $body
    Write-Host "✅ Tạo tài khoản thành công!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📧 Email: teacher@pcm.local" -ForegroundColor Cyan
    Write-Host "🔑 Password: Teacher@123" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Access Token: $($response.accessToken)" -ForegroundColor Yellow
} catch {
    Write-Host "❌ Lỗi: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.Exception.Response
}
