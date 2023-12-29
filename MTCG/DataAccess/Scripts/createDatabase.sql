-- users

CREATE TABLE IF NOT EXISTS users(
    id SERIAL NOT NULL UNIQUE,
    username VARCHAR NOT NULL,
    password VARCHAR NOT NULL,
    bio VARCHAR NOT NULL,
    image VARCHAR NOT NULL,
    currency INT NOT NULL DEFAULT 20,
    score INT NOT NULL DEFAULT 100,
    wins INT NOT NULL DEFAULT 0,
    losses INT NOT NULL DEFAULT 0
);

-- tokens

CREATE TABLE IF NOT EXISTS tokens(
    id SERIAL NOT NULL UNIQUE,
    guid VARCHAR NOT NULL,
    userid INT NOT NULL,
    -- constraints
    CONSTRAINT fkUserId
        FOREIGN KEY(userid)
            REFERENCES users(id)
);

-- elements

CREATE TABLE IF NOT EXISTS elements(
    id SERIAL NOT NULL UNIQUE,
    name VARCHAR NOT NULL
);

-- cardtypes

CREATE TABLE IF NOT EXISTS cardtypes(
    id SERIAL NOT NULL UNIQUE,
    name VARCHAR NOT NULL
);

-- cards

CREATE TABLE IF NOT EXISTS cards(
    id SERIAL NOT NULL UNIQUE,
    elementid INT NOT NULL,
    cardtypeid INT NOT NULL,
    damage INT NOT NULL,
    name VARCHAR NOT NULL,
    -- constraints
    CONSTRAINT fkElementId
        FOREIGN KEY(elementid)
            REFERENCES elements(id),
    CONSTRAINT fkCardtypeId
        FOREIGN KEY(cardtypeid)
            REFERENCES cardtypes(id)
);

-- createdcards

CREATE TABLE IF NOT EXISTS createdcards(
    id SERIAL NOT NULL UNIQUE,
    guid VARCHAR NOT NULL,
    userid INT,
    cardid INT NOT NULL,
    deck BOOL DEFAULT false,
    -- constraints
    CONSTRAINT fkUserId
        FOREIGN KEY(userid)
            REFERENCES users(id),
    CONSTRAINT fkCardId
        FOREIGN KEY(cardid)
            REFERENCES cards(id)
);

-- tradingdeals

CREATE TABLE IF NOT EXISTS tradingdeals(
    id SERIAL NOT NULL UNIQUE,
    guid VARCHAR NOT NULL,
    userid INT NOT NULL,
    cardid INT NOT NULL,
    mindamage INT NOT NULL,
    cardtypeid INT NOT NULL,
    -- constraints
    CONSTRAINT fkUserid
        FOREIGN KEY(userid)
            REFERENCES users(id),
    CONSTRAINT fkCardId
        FOREIGN KEY(cardid)
            REFERENCES createdcards(id),
    CONSTRAINT fkCardtypeId
        FOREIGN KEY(cardtypeid)
            REFERENCES cardtypes(id)
)