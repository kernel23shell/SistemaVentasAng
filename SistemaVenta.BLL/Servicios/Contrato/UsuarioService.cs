using AutoMapper;
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

namespace SistemaVenta.BLL.Servicios.Contrato
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
        public Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(UsuarioDTO modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int id)
        {
            throw new NotImplementedException();
        }
    }
}
