﻿using AutoMapper;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using Microsoft.EntityFrameworkCore;

namespace SistemaVenta.BLL.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        //Task<List<RolDTO>> Lista();

        private readonly IGenericRepository<Usuario> _usuarioRepositorio;
        private readonly IMapper _mapper;

        public UsuarioService(IGenericRepository<Usuario> usuarioRepositorio, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }
        public async Task<List<UsuarioDTO>> Lista()
        {
            try {
                var QueryUsuario = await _usuarioRepositorio.Consultar();
                var listaUsuarios = QueryUsuario.Include(rol => rol.IdRolNavigation).ToList();

                return _mapper.Map<List<UsuarioDTO>>(listaUsuarios);
            }
            catch { throw; }
        }

        public async Task<SesionDTO> ValidarCredenciales(string correo, string clave)
        {
            try
            {
                //
                var QueryUsuario = await _usuarioRepositorio.Consultar(
                u => u.Correo == correo &&
                u.Clave == clave
            );

                if (QueryUsuario.FirstOrDefault() == null)
                    throw new TaskCanceledException("El usuario no existe");

                Usuario devolverUsuario = QueryUsuario.Include(rol => rol.IdRolNavigation).First();

                return _mapper.Map<SesionDTO>(devolverUsuario);
            }
            catch { throw; }
            
        }
        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            try
            {
                var usuariCreado = await _usuarioRepositorio.Crear(_mapper.Map<Usuario>(modelo));

                if (usuariCreado.IdUsuario == 0)
                    throw new TaskCanceledException("El usuario no se pudo crear");

                var query = await _usuarioRepositorio.Consultar(u => u.IdUsuario == usuariCreado.IdUsuario);

                usuariCreado = query.Include(rol => rol.IdRolNavigation).First();

                return _mapper.Map<UsuarioDTO>(usuariCreado);


            }
            catch { throw; }
        }

        public async Task<bool> Editar(UsuarioDTO modelo)
        {
            try {
                var usuarioModelo = _mapper.Map<Usuario>(modelo);
                var usuarioEncontrado = await _usuarioRepositorio.Obtener(u => u.IdUsuario == usuarioModelo.IdUsuario);

                if(usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                usuarioEncontrado.NombreCompleto = usuarioModelo.NombreCompleto;
                usuarioEncontrado.Correo = usuarioModelo.Correo;
                usuarioEncontrado.IdRol = usuarioModelo.IdRol;
                usuarioEncontrado.Clave = usuarioModelo.Clave;
                usuarioEncontrado.EsActivo = usuarioModelo.EsActivo;

                bool respuesta = await _usuarioRepositorio.Editar(usuarioEncontrado);
                if(!respuesta)
                    throw new TaskCanceledException("No se pudo editar");
                return respuesta;

            } catch { throw; }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var usuarioEncontrado = await _usuarioRepositorio.Obtener(u => u.IdUsuario == id);

                if(usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                bool respuesta = await _usuarioRepositorio.Eliminar(usuarioEncontrado);

                if(!respuesta)
                    throw new TaskCanceledException("El usuario no se pudo eliminar");

                return respuesta;
            }
            catch { throw; }
        }
    }
}
