-- Insert sample currencies
INSERT INTO Currency (Id, Name, Rate) VALUES
                                          (1, 'US Dollar', 1.0),
                                          (2, 'Euro', 0.85),
                                          (3, 'Japanese Yen', 110.0);

-- Insert sample countries
INSERT INTO Country (Id, Name) VALUES
                                   (1, 'United States'),
                                   (2, 'Germany'),
                                   (3, 'Japan'),
                                   (4, 'France');

-- Link countries to currencies
INSERT INTO Currency_Country (Country_Id, Currency_Id) VALUES
                                                           (1, 1), -- United States uses US Dollar
                                                           (2, 2), -- Germany uses Euro
                                                           (3, 3), -- Japan uses Yen
                                                           (4, 2); -- France uses Euro
