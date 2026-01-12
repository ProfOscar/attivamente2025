-- =====================================================================
-- SEED DATI INIZIALI
-- Ruoli + Utenti
-- =====================================================================

DECLARE @PasswordHash NVARCHAR(512) = 'HASH_FAKE_123456789'; -- valore segnaposto


-- =====================================================================
-- RUOLI (seed statico)
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM Ruoli)
BEGIN
    INSERT INTO Ruoli (Nome) VALUES
        (N'Admin'),
        (N'Coordinatore'),
        (N'Volontario'),
        (N'Guest');
END


-- =====================================================================
-- UTENTI (seed dinamico)
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM Utenti)
BEGIN
    -- Liste di nomi e cognomi da combinare casualmente
    DECLARE @Nomi TABLE (Nome NVARCHAR(100));
    INSERT INTO @Nomi VALUES
    (N'Giuseppe'),(N'Maria'),(N'Luigi'),(N'Anna'),(N'Francesco'),
    (N'Lucia'),(N'Giorgio'),(N'Giulia'),(N'Antonio'),(N'Carla'),
    (N'Enrico'),(N'Paola'),(N'Marco'),(N'Laura'),(N'Pietro'),
    (N'Elena'),(N'Matteo'),(N'Chiara'),(N'Roberto'),(N'Angela'),
    (N'Vittorio'),(N'Claudia'),(N'Leonardo'),(N'Silvia'),(N'Alessandro'),
    (N'Federica'),(N'Gabriele'),(N'Valentina'),(N'Carlo'),(N'Beatrice'),
    (N'Andrea'),(N'Margherita'),(N'Nicola'),(N'Ilaria'),(N'Paolo'),
    (N'Noemi'),(N'Stefano'),(N'Serena'),(N'Corrado'),(N'Lisa'),
    (N'Napoleone'),(N'Giovanni'),(N'Virginia'),(N'Umberto'),(N'Elisa'),
    (N'Dante'),(N'Alighieri'),(N'Rita'),(N'Aldo'),(N'Nilde');

    DECLARE @Cognomi TABLE (Cognome NVARCHAR(100));
    INSERT INTO @Cognomi VALUES
    (N'Bianchi'),(N'Rossi'),(N'Verdi'),(N'Neri'),(N'Esposito'),
    (N'Russo'),(N'Ferrari'),(N'Gallo'),(N'Greco'),(N'Conti'),
    (N'Costa'),(N'Giordano'),(N'Mancini'),(N'Lombardi'),(N'Barbieri'),
    (N'Fontana'),(N'Moretti'),(N'Mariani'),(N'Rizzo'),(N'Caruso'),
    (N'Orlando'),(N'Piras'),(N'Del Piero'),(N'Garibaldi'),(N'Cavour'),
    (N'Bonaparte'),(N'Galilei'),(N'Medici'),(N'Manzoni'),(N'Gramsci'),
    (N'Levi'),(N'Pasolini'),(N'Pavese'),(N'Calvino'),(N'Fermi'),
    (N'Rubbia'),(N'Pertini'),(N'Einstein'),(N'Davinci'),(N'Giolitti'),
    (N'Moro'),(N'Berlinguer'),(N'Spadolini'),(N'Zanetti'),(N'Carli'),
    (N'Piacentini'),(N'Longo'),(N'Spinelli'),(N'Sforza'),(N'Del Vecchio');

    -- Creazione utenti casuali
    DECLARE @i INT = 1;
    WHILE @i <= 300
    BEGIN
        DECLARE @Nome NVARCHAR(100);
        DECLARE @Cognome NVARCHAR(100);
        DECLARE @RuoloId INT;
        DECLARE @Email NVARCHAR(255);

        SELECT TOP 1 @Nome = Nome FROM @Nomi ORDER BY NEWID();
        SELECT TOP 1 @Cognome = Cognome FROM @Cognomi ORDER BY NEWID();
        SET @RuoloId = (ABS(CHECKSUM(NEWID())) % 4) + 1;
        SET @Email = LOWER(@Nome + '.' + REPLACE(@Cognome,' ','') + CAST(@i AS NVARCHAR) + '@attivamente.it');

        INSERT INTO Utenti (Nome, Cognome, Email, PasswordHash, RuoloId)
        VALUES (@Nome, @Cognome, @Email, @PasswordHash, @RuoloId);

        SET @i += 1;
    END
END

PRINT '4 ruoli e 300 utenti inseriti correttamente.';