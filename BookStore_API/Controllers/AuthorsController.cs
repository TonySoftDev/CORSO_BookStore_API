﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Raccolta di endpoints per interagire con la tabella Authors del database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Legge e restituisce l'elenco degli autori
        /// </summary>
        /// <returns>List of AuthorDTO</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Tentativo di lettura degli Autori.");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo($"{location}: Lettura riuscita.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Legge e restituisce un autore specifico
        /// </summary>
        /// <param name="id"></param>
        /// <returns>AuthorDTO object</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Tentativo di lettura di un autore con id: {id}");
                var author = await _authorRepository.FindById(id);
                if( author == null )
                {
                    _logger.LogWarn($"{location}: Non è stato trovato un autore con id: {id}");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"{location}: Lettura riuscita.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Crea e restitisce un nuovo autore
        /// </summary>
        /// <param name="authorCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorCreateDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Tentativo di scrittura di un autore");
                if (authorCreateDTO == null)
                {
                    _logger.LogWarn($"{location}: La Request è vuota");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: I dati dell'autore sono incompleti");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorCreateDTO);
                var isSuccess = await _authorRepository.Create(author);
                if(!isSuccess)
                {
                    return InternalError($"{location}: Creazione autore fallita.");
                }
                _logger.LogInfo($"{location}: Autore creato con successo.");
                return Created("Create", new { author });
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Aggiorna un autore esistente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorUpdateDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorUpdateDTO )
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Tentativo di aggiornamento di un autore");
                if (id < 1 || authorUpdateDTO == null || id != authorUpdateDTO.Id)
                {
                    _logger.LogWarn($"{location}: I dati della Request non sono validi");
                    return BadRequest(ModelState);
                }
                var isExists = await _authorRepository.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location}: L'autore da modificare non esiste");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: I dati dell'autore sono incompleti");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorUpdateDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Aggiornamento autore fallita.");
                }
                _logger.LogInfo($"{location}: Autore aggiornato con successo.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Elimina un autore dal database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Tentativo di eliminazione di un autore");
                if (id < 1)
                {
                    _logger.LogWarn($"{location}: L'id passato non è valido");
                    return BadRequest();
                }
                var isExists = await _authorRepository.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location}: L'autore da cancellare non esiste");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Eliminazione autore fallita.");
                }
                _logger.LogInfo($"{location}: Autore eliminato con successo.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Si è verificato un errore imprevisto. Prego, contattare l'Amministratore del Sistema."); //Intrnal server error
        }
    }
}
