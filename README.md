# GingCase

Ufak notlar:

-İstenilen iki mekanik "ChuteScene" ve "BalloonScene" isimli sahnelerde bulunuyor.

-Chute sahnesinde input olarak sadece çizim yapabiliyoruz.

-Chute için ilk başta runtime mesh generation denedim. Düşündüğümden daha zor çıktı ve çözülmesi çok zor bazı problemler vardı (UV normallerinin çizilen yöne göre hangi doğrultuya bakması gerektiği gibi). Ama yine de bu çözümü kodda yorum satırı olarak bıraktım. Yapısı oldukça çirkin gözükse de işe yarıyor :).

-Balon sahnesinde otomatik olarak 20 adet balon spawnlanıyor (max sayı ve spawnrate BalloonSpawner'dan ayarlanabilinir)

-Ayrıca bu shaneye input da koydum. Joystick ile sağa ve sola doğru hareket edebiliyorsunuz(Hissiyat testi için).

-3. oyun için olan UML diagramı Asset klasörü içinde.
