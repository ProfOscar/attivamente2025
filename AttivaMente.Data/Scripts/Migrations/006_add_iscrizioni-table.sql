CREATE TABLE Iscrizioni
(
	Id INT IDENTITY PRIMARY KEY,
	UtenteId INT NOT NULL,
	Anno INT NOT NULL,
	Tipo NVARCHAR(10) NOT NULL, -- 'Nuova' | 'Rinnovo'
	DataIscrizione DATE NOT NULL DEFAULT GETDATE(),

	CONSTRAINT FK_Iscrizioni_Utenti
		FOREIGN KEY (UtenteId) REFERENCES Utenti(Id),

	CONSTRAINT UQ_Iscrizioni_Utente_Anno
		UNIQUE (UtenteId, Anno),

	CONSTRAINT CK_Iscrizioni_Tipo
		CHECK (Tipo IN (N'Nuova', N'Rinnovo'))
);
