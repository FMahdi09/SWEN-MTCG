-- users

CREATE TABLE IF NOT EXISTS users(
    id SERIAL NOT NULL UNIQUE,
    username VARCHAR NOT NULL,
    password VARCHAR NOT NULL,
    bio VARCHAR NOT NULL,
    image VARCHAR NOT NULL,
    currency INT NOT NULL DEFAULT 20
);

-- tokens

CREATE TABLE IF NOT EXISTS tokens(
    id SERIAL NOT NULL UNIQUE,
    guid VARCHAR NOT NULL,
    userId INT NOT NULL,
    CONSTRAINT fkUserId
        FOREIGN KEY(userId)
            REFERENCES users(id)
);