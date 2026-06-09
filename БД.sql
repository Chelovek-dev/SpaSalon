-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Хост: 127.0.0.1:3306
-- Время создания: Июн 10 2026 г., 00:20
-- Версия сервера: 5.7.39
-- Версия PHP: 8.1.9

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `курсовая`
--

-- --------------------------------------------------------

--
-- Структура таблицы `мастера`
--

CREATE TABLE `мастера` (
  `код мастера` int(10) NOT NULL,
  `фио` varchar(50) NOT NULL,
  `телефон` varchar(20) NOT NULL,
  `пароль` varchar(255) NOT NULL,
  `должность` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `мастера`
--

INSERT INTO `мастера` (`код мастера`, `фио`, `телефон`, `пароль`, `должность`) VALUES
(1, 'Анна Иванова', '+7 (999) 111-22-33', '123', 'Старший мастер'),
(2, 'Дмитрий Петров', '+7 (999) 222-33-44', '123', 'Мастер'),
(3, 'Елена Сидорова', '+7 (999) 333-44-55', '123', 'Мастер'),
(4, 'Администратор', '+7 (999) 000-11-22', 'admin', 'Администратор');

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `мастера`
--
ALTER TABLE `мастера`
  ADD PRIMARY KEY (`код мастера`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `мастера`
--
ALTER TABLE `мастера`
  MODIFY `код мастера` int(10) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
