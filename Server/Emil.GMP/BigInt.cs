using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security;

namespace Emil.GMP
{
	public unsafe sealed class BigInt : IEquatable<BigInt>, IComparable<BigInt>, IComparable, ICloneable, IConvertible
	{
		#region Interop

		[StructLayout(LayoutKind.Sequential)]
		private struct MpzValue
		{
			public int ChunksAllocatedCount;
			public int ChunkCount;
			public uint* Data;
		}

		private const string MPZ_LIBRARY_PATH = @"libgmp-3.dll";

		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_init(ref MpzValue x);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_init2(ref MpzValue x, uint n);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_init_set(ref MpzValue rop, ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_init_set_si(ref MpzValue rop, int op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_init_set_ui(ref MpzValue rop, uint op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_init_set_d(ref MpzValue rop, double op);
		[DllImport(MPZ_LIBRARY_PATH, CharSet = CharSet.Ansi)]
		private static extern int __gmpz_init_set_str(ref MpzValue rop, string str, int @base);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_clear(ref MpzValue x);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_import(ref MpzValue rop, uint count, int order, uint size, int endian, uint nails, void* op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void* __gmpz_export(void* rop, uint* countp, int order, uint size, int endian, uint nails, ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_neg(ref MpzValue rop, ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_abs(ref MpzValue rop, ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_sizeinbase(ref MpzValue op, int @base);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_cmp(ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_cmp_si(ref MpzValue op1, int op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_cmp_ui(ref MpzValue op1, uint op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_cmp_d(ref MpzValue op1, double op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_cmpabs(ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_cmpabs_ui(ref MpzValue op1, uint op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_cmpabs_d(ref MpzValue op1, double op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_powm(ref MpzValue rop, ref MpzValue @base, ref MpzValue exp, ref MpzValue mod);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_powm_ui(ref MpzValue rop, ref MpzValue @base, uint exp, ref MpzValue mod);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_pow_ui(ref MpzValue rop, ref MpzValue @base, uint exp);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_ui_pow_ui(ref MpzValue rop, uint @base, uint exp);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_add(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_add_ui(ref MpzValue rop, ref MpzValue op1, uint op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_sub(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_ui_sub(ref MpzValue rop, uint op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_sub_ui(ref MpzValue rop, ref MpzValue op1, uint op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_mul(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_mul_si(ref MpzValue rop, ref MpzValue op1, int op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_mul_ui(ref MpzValue rop, ref MpzValue op1, uint op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_mul_2exp(ref MpzValue rop, ref MpzValue op1, uint op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_addmul(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_addmul_ui(ref MpzValue rop, ref MpzValue op1, uint op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_submul(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_submul_ui(ref MpzValue rop, ref MpzValue op1, uint op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_tdiv_q(ref MpzValue q, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_tdiv_r(ref MpzValue r, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_tdiv_qr(ref MpzValue q, ref MpzValue r, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_tdiv_q_ui(ref MpzValue q, ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_tdiv_r_ui(ref MpzValue r, ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_tdiv_qr_ui(ref MpzValue q, ref MpzValue r, ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_tdiv_ui(ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_tdiv_q_2exp(ref MpzValue q, ref MpzValue n, uint b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_tdiv_r_2exp(ref MpzValue r, ref MpzValue n, uint b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_cdiv_q(ref MpzValue q, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_cdiv_r(ref MpzValue r, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_cdiv_qr(ref MpzValue q, ref MpzValue r, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_cdiv_q_ui(ref MpzValue q, ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_cdiv_r_ui(ref MpzValue r, ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_cdiv_qr_ui(ref MpzValue q, ref MpzValue r, ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_cdiv_ui(ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_cdiv_q_2exp(ref MpzValue q, ref MpzValue n, uint b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_cdiv_r_2exp(ref MpzValue r, ref MpzValue n, uint b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_fdiv_q(ref MpzValue q, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_fdiv_r(ref MpzValue r, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_fdiv_qr(ref MpzValue q, ref MpzValue r, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_fdiv_q_ui(ref MpzValue q, ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_fdiv_r_ui(ref MpzValue r, ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_fdiv_qr_ui(ref MpzValue q, ref MpzValue r, ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_fdiv_ui(ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_fdiv_q_2exp(ref MpzValue q, ref MpzValue n, uint b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_fdiv_r_2exp(ref MpzValue r, ref MpzValue n, uint b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_mod(ref MpzValue r, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_divexact(ref MpzValue q, ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_divexact_ui(ref MpzValue q, ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_divisible_p(ref MpzValue n, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_divisible_ui_p(ref MpzValue n, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_divisible_2exp_p(ref MpzValue n, uint b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_congruent_p(ref MpzValue n, ref MpzValue c, ref MpzValue d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_congruent_ui_p(ref MpzValue n, uint c, uint d);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_congruent_2exp_p(ref MpzValue n, ref MpzValue c, uint b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_invert(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_and(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_ior(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_xor(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_com(ref MpzValue rop, ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_popcount(ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_hamdist(ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_scan0(ref MpzValue op, uint starting_bit);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_scan1(ref MpzValue op, uint starting_bit);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_setbit(ref MpzValue rop, uint bit_index);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_clrbit(ref MpzValue rop, uint bit_index);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_combit(ref MpzValue rop, uint bit_index);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_tstbit(ref MpzValue rop, uint bit_index);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_get_ui(ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_get_si(ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern double __gmpz_get_d(ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, CharSet = CharSet.Ansi)]
		private static extern void* __gmpz_get_str(StringBuilder str, int @base, ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_root(ref MpzValue rop, ref MpzValue op, uint n);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_rootrem(ref MpzValue root, ref MpzValue rem, ref MpzValue u, uint n);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_sqrt(ref MpzValue rop, ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_sqrtrem(ref MpzValue rop1, ref MpzValue rop2, ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_perfect_power_p(ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_perfect_square_p(ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_probab_prime_p(ref MpzValue n, int reps);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_nextprime(ref MpzValue rop, ref MpzValue op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_gcd(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_gcd_ui(ref MpzValue rop, ref MpzValue op1, uint op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_gcdext(ref MpzValue g, ref MpzValue s, ref MpzValue t, ref MpzValue a, ref MpzValue b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_gcdext(ref MpzValue g, ref MpzValue s, MpzValue* t, ref MpzValue a, ref MpzValue b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_lcm(ref MpzValue rop, ref MpzValue op1, ref MpzValue op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_lcm_ui(ref MpzValue rop, ref MpzValue op1, uint op2);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_jacobi(ref MpzValue a, ref MpzValue b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_legendre(ref MpzValue a, ref MpzValue p);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_kronecker(ref MpzValue a, ref MpzValue b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_kronecker_si(ref MpzValue a, int b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_kronecker_ui(ref MpzValue a, uint b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_si_kronecker(int a, ref MpzValue b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern int __gmpz_ui_kronecker(uint a, ref MpzValue b);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern uint __gmpz_remove(ref MpzValue rop, ref MpzValue op, ref MpzValue f);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_fac_ui(ref MpzValue rop, uint op);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_bin_ui(ref MpzValue rop, ref MpzValue n, uint k);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_bin_uiui(ref MpzValue rop, uint n, uint k);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_fib_ui(ref MpzValue fn, uint n);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_fib2_ui(ref MpzValue fn, ref MpzValue fnsub1, uint n);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_lucnum_ui(ref MpzValue ln, uint n);
		[DllImport(MPZ_LIBRARY_PATH, SetLastError = false)]
		private static extern void __gmpz_lucnum2_ui(ref MpzValue ln, ref MpzValue lnsub1, uint n);

		#endregion

		#region Data

		private const int s_defaultStringBase = 10;
		private const int s_chunkBitLength = 32;

		private MpzValue InternalValue;

		#endregion

		#region Predefined Values

		public static readonly BigInt NegativeTen = new BigInt(-10);
		public static readonly BigInt NegativeThree = new BigInt(-3);
		public static readonly BigInt NegativeTwo = new BigInt(-2);
		public static readonly BigInt NegativeOne = new BigInt(-1);
		public static readonly BigInt Zero = new BigInt(0);
		public static readonly BigInt One = new BigInt(1);
		public static readonly BigInt Two = new BigInt(2);
		public static readonly BigInt Three = new BigInt(3);
		public static readonly BigInt Ten = new BigInt(10);

		#endregion

		#region Consructors

		public BigInt()
		{
			__gmpz_init(ref this.InternalValue);
		}

		public BigInt(BigInt other)
		{
			__gmpz_init_set(ref this.InternalValue, ref other.InternalValue);
		}

		~BigInt()
		{
			__gmpz_clear(ref this.InternalValue);
			//this.InternalValue = new MpzValue();
		}

		public BigInt(int value)
		{
			__gmpz_init_set_si(ref this.InternalValue, value);
		}

		public BigInt(uint value)
		{
			__gmpz_init_set_ui(ref this.InternalValue, value);
		}

		public BigInt(uint value, int sign)
		{
			Debug.Assert(value == 0 && sign == 0 || value != 0 && sign != 0);

			if(sign >= 0)
			{
				__gmpz_init_set_ui(ref this.InternalValue, value);
			}
			else
			{
				__gmpz_init2(ref this.InternalValue, 32);
				Debug.Assert(this.InternalValue.ChunksAllocatedCount == 1);
				Debug.Assert(this.InternalValue.ChunkCount == 0);

				this.InternalValue.ChunkCount = -1;

				this.InternalValue.Data[0] = (uint)value;

				Debug.Assert(this == value);
			}
		}

		public BigInt(float value)
			: this((double)value)
		{
		}

		public BigInt(double value)
		{
			Debug.Assert(!double.IsNaN(value) && !double.IsInfinity(value));

			__gmpz_init_set_d(ref this.InternalValue, value);
		}

		public BigInt(decimal value)
			: this((double)value)
		{
		}

		public BigInt(long value)
		{
			if(value > 0)
			{
				if(value <= (uint)int.MaxValue)
				{
					__gmpz_init2(ref this.InternalValue, 32);
					Debug.Assert(this.InternalValue.ChunksAllocatedCount == 1);
					Debug.Assert(this.InternalValue.ChunkCount == 0);
					uint* data = this.InternalValue.Data;
					data[0] = (uint)value;
					this.InternalValue.ChunkCount = 1;
				}
				else
				{
					__gmpz_init2(ref this.InternalValue, 64);
					Debug.Assert(this.InternalValue.ChunksAllocatedCount == 2);
					Debug.Assert(this.InternalValue.ChunkCount == 0);
					uint* data = this.InternalValue.Data;
					data[0] = (uint)value;
					data[1] = (uint)(value >> 32);
					this.InternalValue.ChunkCount = 2;
				}
			}
			else if(value == 0)
			{
				__gmpz_init(ref this.InternalValue);
			}
			else // value < 0
			{
				ulong absValue = (ulong)(-value);

				if(absValue <= (uint)int.MaxValue)
				{
					__gmpz_init2(ref this.InternalValue, 32);
					Debug.Assert(this.InternalValue.ChunksAllocatedCount == 1);
					Debug.Assert(this.InternalValue.ChunkCount == 0);
					uint* data = this.InternalValue.Data;
					data[0] = (uint)absValue;
					this.InternalValue.ChunkCount = -1;
				}
				else
				{
					__gmpz_init2(ref this.InternalValue, 64);
					Debug.Assert(this.InternalValue.ChunksAllocatedCount == 2);
					Debug.Assert(this.InternalValue.ChunkCount == 0);
					uint* data = this.InternalValue.Data;
					data[0] = (uint)absValue;
					data[1] = (uint)(absValue >> 32);
					this.InternalValue.ChunkCount = -2;
				}
			}

			Debug.Assert(this == value);
		}

		public BigInt(ulong value)
		{
			if(value == 0)
			{
				__gmpz_init(ref this.InternalValue);
			}
			else
			{
				if(value <= (uint)int.MaxValue)
				{
					__gmpz_init2(ref this.InternalValue, 32);
					Debug.Assert(this.InternalValue.ChunksAllocatedCount == 1);
					Debug.Assert(this.InternalValue.ChunkCount == 0);
					uint* data = this.InternalValue.Data;
					data[0] = (uint)value;
					this.InternalValue.ChunkCount = 1;
				}
				else
				{
					__gmpz_init2(ref this.InternalValue, 64);
					Debug.Assert(this.InternalValue.ChunksAllocatedCount == 2);
					Debug.Assert(this.InternalValue.ChunkCount == 0);
					uint* data = this.InternalValue.Data;
					data[0] = (uint)value;
					data[1] = (uint)(value >> 32);
					this.InternalValue.ChunkCount = 2;
				}
			}
		}

		public BigInt(ulong value, int sign)
		{
			Debug.Assert(value == 0 && sign == 0 || value != 0 && sign != 0);

			if(sign == 0)
			{
				__gmpz_init(ref this.InternalValue);
			}
			else
			{
				__gmpz_init2(ref this.InternalValue, 64);
				Debug.Assert(this.InternalValue.ChunksAllocatedCount == 2);
				Debug.Assert(this.InternalValue.ChunkCount == 0);

				bool isLength1 = (uint)value == value;

				if(sign > 0)
					this.InternalValue.ChunkCount = isLength1 ? 1 : 2;
				else
					this.InternalValue.ChunkCount = isLength1 ? -1 : -2;

				uint* data = this.InternalValue.Data;
				data[0] = (uint)value;
				data[1] = (uint)(value >> 32);

				Debug.Assert(this == value);
			}
		}

		public BigInt(byte[] value, int sign)
		{
			__gmpz_init(ref this.InternalValue);

			if(sign == 0)
				return;

			fixed(byte* data = value)
				__gmpz_import(ref this.InternalValue, (uint)value.Length, -1, sizeof(byte), 0, 0, data);

			if(sign < 0)
				this.InternalValue.ChunkCount = -this.InternalValue.ChunkCount;
		}

		public BigInt(byte[] value)
			: this(value, IsZero(value) ? 0 : 1)
		{
		}

		public BigInt(uint[] value, int sign)
		{
			__gmpz_init(ref this.InternalValue);

			if(sign == 0)
				return;

			fixed(uint* data = value)
				__gmpz_import(ref this.InternalValue, (uint)value.Length, -1, sizeof(uint), 0, 0, data);

			if(sign < 0)
				this.InternalValue.ChunkCount = -this.InternalValue.ChunkCount;
		}

		public BigInt(uint[] value)
			: this(value, IsZero(value) ? 0 : 1)
		{
		}

		public BigInt(string s)
			: this(s, s_defaultStringBase)
		{
		}

		public BigInt(string s, int @base)
		{
			if(@base < 2 || @base > 62)
				throw new ArgumentOutOfRangeException();

			int status = __gmpz_init_set_str(ref this.InternalValue, s, @base);

			if(status != 0)
			{
				__gmpz_clear(ref this.InternalValue);
				this.InternalValue = new MpzValue();
				throw new FormatException();
			}
		}


		#endregion

		#region Operators

		public static BigInt operator -(BigInt x)
		{
			BigInt z = new BigInt();
			__gmpz_neg(ref z.InternalValue, ref x.InternalValue);
			return z;
		}

		public static BigInt operator ~(BigInt x)
		{
			BigInt z = new BigInt();
			__gmpz_com(ref z.InternalValue, ref x.InternalValue);
			return z;
		}

		public static BigInt operator +(BigInt x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_add(ref z.InternalValue, ref x.InternalValue, ref y.InternalValue);
			return z;
		}

		public static BigInt operator +(BigInt x, int y)
		{
			BigInt z = new BigInt();

			if(y >= 0)
			{
				__gmpz_add_ui(ref z.InternalValue, ref x.InternalValue, (uint)y);
			}
			else
			{
				__gmpz_sub_ui(ref z.InternalValue, ref x.InternalValue, (uint)(-y));
			}

			return z;
		}

		public static BigInt operator +(int x, BigInt y)
		{
			BigInt z = new BigInt();
			if(x >= 0)
			{
				__gmpz_add_ui(ref z.InternalValue, ref y.InternalValue, (uint)x);
			}
			else
			{
				__gmpz_sub_ui(ref z.InternalValue, ref y.InternalValue, (uint)(-x));
			}

			return z;
		}

		public static BigInt operator +(BigInt x, uint y)
		{
			BigInt z = new BigInt();
			__gmpz_add_ui(ref z.InternalValue, ref x.InternalValue, y);
			return z;
		}

		public static BigInt operator +(uint x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_add_ui(ref z.InternalValue, ref y.InternalValue, x);
			return z;
		}

		public static BigInt operator -(BigInt x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_sub(ref z.InternalValue, ref x.InternalValue, ref y.InternalValue);
			return z;
		}

		public static BigInt operator -(int x, BigInt y)
		{
			BigInt z = new BigInt();

			if(x >= 0)
			{
				__gmpz_ui_sub(ref z.InternalValue, (uint)x, ref y.InternalValue);
			}
			else
			{
				__gmpz_add_ui(ref z.InternalValue, ref y.InternalValue, (uint)(-x));
				z.InternalValue.ChunkCount = -z.InternalValue.ChunkCount;
			}

			return z;
		}

		public static BigInt operator -(BigInt x, int y)
		{
			BigInt z = new BigInt();

			if(y >= 0)
			{
				__gmpz_sub_ui(ref z.InternalValue, ref x.InternalValue, (uint)y);
			}
			else
			{
				__gmpz_add_ui(ref z.InternalValue, ref x.InternalValue, (uint)(-y));

			}

			return z;
		}

		public static BigInt operator -(uint x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_ui_sub(ref z.InternalValue, x, ref y.InternalValue);
			return z;
		}

		public static BigInt operator -(BigInt x, uint y)
		{
			BigInt z = new BigInt();
			__gmpz_sub_ui(ref z.InternalValue, ref x.InternalValue, y);
			return z;
		}

		public static BigInt operator ++(BigInt x)
		{
			BigInt z = new BigInt();
			__gmpz_add_ui(ref z.InternalValue, ref x.InternalValue, 1);
			return z;
		}

		public static BigInt operator --(BigInt x)
		{
			BigInt z = new BigInt();
			__gmpz_sub_ui(ref z.InternalValue, ref x.InternalValue, 1);
			return z;
		}

		public static BigInt operator *(BigInt x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_mul(ref z.InternalValue, ref x.InternalValue, ref y.InternalValue);
			return z;
		}

		public static BigInt operator *(int x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_mul_si(ref z.InternalValue, ref y.InternalValue, x);
			return z;
		}

		public static BigInt operator *(BigInt x, int y)
		{
			BigInt z = new BigInt();
			__gmpz_mul_si(ref z.InternalValue, ref x.InternalValue, y);
			return z;
		}

		public static BigInt operator *(uint x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_mul_ui(ref z.InternalValue, ref y.InternalValue, x);
			return z;
		}

		public static BigInt operator *(BigInt x, uint y)
		{
			BigInt z = new BigInt();
			__gmpz_mul_ui(ref z.InternalValue, ref x.InternalValue, y);
			return z;
		}

		public static BigInt operator /(BigInt x, BigInt y)
		{
			BigInt quotient = new BigInt();
			__gmpz_tdiv_q(ref quotient.InternalValue, ref x.InternalValue, ref y.InternalValue);
			return quotient;
		}

		public static BigInt operator /(BigInt x, int y)
		{
			BigInt quotient = new BigInt();

			if(y >= 0)
			{
				__gmpz_tdiv_q_ui(ref quotient.InternalValue, ref x.InternalValue, (uint)y);
			}
			else
			{
				__gmpz_tdiv_q_ui(ref quotient.InternalValue, ref x.InternalValue, (uint)(-y));
				quotient.InternalValue.ChunkCount = -quotient.InternalValue.ChunkCount;
			}

			return quotient;
		}

		public static BigInt operator /(BigInt x, uint y)
		{
			BigInt quotient = new BigInt();
			__gmpz_tdiv_q_ui(ref quotient.InternalValue, ref x.InternalValue, y);
			return quotient;
		}

		public static BigInt operator &(BigInt x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_and(ref z.InternalValue, ref x.InternalValue, ref y.InternalValue);
			return z;
		}

		public static BigInt operator |(BigInt x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_ior(ref z.InternalValue, ref x.InternalValue, ref y.InternalValue);
			return z;
		}

		public static BigInt operator ^(BigInt x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_xor(ref z.InternalValue, ref x.InternalValue, ref y.InternalValue);
			return z;
		}

		public static BigInt operator %(BigInt x, BigInt mod)
		{
			BigInt z = new BigInt();
			__gmpz_mod(ref z.InternalValue, ref x.InternalValue, ref mod.InternalValue);
			return z;
		}

		public static BigInt operator %(BigInt x, int mod)
		{
			if(mod < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			__gmpz_fdiv_r_ui(ref z.InternalValue, ref x.InternalValue, (uint)mod);
			return z;
		}

		public static BigInt operator %(BigInt x, uint mod)
		{
			BigInt z = new BigInt();
			__gmpz_fdiv_r_ui(ref z.InternalValue, ref x.InternalValue, mod);
			return z;
		}

		public static bool operator <(BigInt x, BigInt y)
		{
			return x.CompareTo(y) < 0;
		}

		public static bool operator <(int x, BigInt y)
		{
			if(x == 0)
				return 0 < y.InternalValue.ChunkCount;

			return y.CompareTo(x) > 0;
		}

		public static bool operator <(BigInt x, int y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount < 0;

			return x.CompareTo(y) < 0;
		}

		public static bool operator <(uint x, BigInt y)
		{
			if(x == 0)
				return 0 < y.InternalValue.ChunkCount;

			return y.CompareTo(x) > 0;
		}

		public static bool operator <(BigInt x, uint y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount < 0;

			return x.CompareTo(y) < 0;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator <(long x, BigInt y)
		{
			return (BigInt)x < y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator <(BigInt x, long y)
		{
			return x < (BigInt)y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator <(ulong x, BigInt y)
		{
			return (BigInt)x < y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator <(BigInt x, ulong y)
		{
			return x < (BigInt)y;
		}

		public static bool operator <(float x, BigInt y)
		{
			if(x == 0)
				return 0 < y.InternalValue.ChunkCount;

			return y.CompareTo(x) > 0;
		}

		public static bool operator <(BigInt x, float y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount < 0;

			return x.CompareTo(y) < 0;
		}

		public static bool operator <(double x, BigInt y)
		{
			if(x == 0)
				return 0 < y.InternalValue.ChunkCount;

			return y.CompareTo(x) > 0;
		}

		public static bool operator <(BigInt x, double y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount < 0;

			return x.CompareTo(y) < 0;
		}

		public static bool operator <(decimal x, BigInt y)
		{
			if(x == 0)
				return 0 < y.InternalValue.ChunkCount;

			return y.CompareTo(x) > 0;
		}

		public static bool operator <(BigInt x, decimal y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount < 0;

			return x.CompareTo(y) < 0;
		}

		public static bool operator <=(BigInt x, BigInt y)
		{
			return x.CompareTo(y) <= 0;
		}

		public static bool operator <=(int x, BigInt y)
		{
			if(x == 0)
				return 0 <= y.InternalValue.ChunkCount;

			return y.CompareTo(x) >= 0;
		}

		public static bool operator <=(BigInt x, int y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount <= 0;

			return x.CompareTo(y) <= 0;
		}

		public static bool operator <=(uint x, BigInt y)
		{
			if(x == 0)
				return 0 <= y.InternalValue.ChunkCount;

			return y.CompareTo(x) >= 0;
		}

		public static bool operator <=(BigInt x, uint y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount <= 0;

			return x.CompareTo(y) <= 0;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator <=(long x, BigInt y)
		{
			return (BigInt)x <= y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator <=(BigInt x, long y)
		{
			return x <= (BigInt)y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator <=(ulong x, BigInt y)
		{
			return (BigInt)x <= y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator <=(BigInt x, ulong y)
		{
			return x <= (BigInt)y;
		}

		public static bool operator <=(float x, BigInt y)
		{
			if(x == 0)
				return 0 <= y.InternalValue.ChunkCount;

			return y.CompareTo(x) >= 0;
		}

		public static bool operator <=(BigInt x, float y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount <= 0;

			return x.CompareTo(y) <= 0;
		}

		public static bool operator <=(double x, BigInt y)
		{
			if(x == 0)
				return 0 <= y.InternalValue.ChunkCount;

			return y.CompareTo(x) >= 0;
		}

		public static bool operator <=(BigInt x, double y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount <= 0;

			return x.CompareTo(y) <= 0;
		}

		public static bool operator <=(decimal x, BigInt y)
		{
			if(x == 0)
				return 0 <= y.InternalValue.ChunkCount;

			return y.CompareTo(x) >= 0;
		}

		public static bool operator <=(BigInt x, decimal y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount <= 0;

			return x.CompareTo(y) <= 0;
		}

		public static bool operator >(BigInt x, BigInt y)
		{
			return x.CompareTo(y) > 0;
		}

		public static bool operator >(int x, BigInt y)
		{
			if(x == 0)
				return 0 > y.InternalValue.ChunkCount;

			return y.CompareTo(x) < 0;
		}

		public static bool operator >(BigInt x, int y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount > 0;

			return x.CompareTo(y) > 0;
		}

		public static bool operator >(uint x, BigInt y)
		{
			if(x == 0)
				return 0 > y.InternalValue.ChunkCount;

			return y.CompareTo(x) < 0;
		}

		public static bool operator >(BigInt x, uint y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount > 0;

			return x.CompareTo(y) > 0;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator >(long x, BigInt y)
		{
			return (BigInt)x > y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator >(BigInt x, long y)
		{
			return x > (BigInt)y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator >(ulong x, BigInt y)
		{
			return (BigInt)x > y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator >(BigInt x, ulong y)
		{
			return x > (BigInt)y;
		}

		public static bool operator >(float x, BigInt y)
		{
			if(x == 0)
				return 0 > y.InternalValue.ChunkCount;

			return y.CompareTo(x) < 0;
		}

		public static bool operator >(BigInt x, float y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount > 0;

			return x.CompareTo(y) > 0;
		}

		public static bool operator >(double x, BigInt y)
		{
			if(x == 0)
				return 0 > y.InternalValue.ChunkCount;

			return y.CompareTo(x) < 0;
		}

		public static bool operator >(BigInt x, double y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount > 0;

			return x.CompareTo(y) > 0;
		}

		public static bool operator >(decimal x, BigInt y)
		{
			if(x == 0)
				return 0 > y.InternalValue.ChunkCount;

			return y.CompareTo(x) < 0;
		}

		public static bool operator >(BigInt x, decimal y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount > 0;

			return x.CompareTo(y) > 0;
		}

		public static bool operator >=(BigInt x, BigInt y)
		{
			return x.CompareTo(y) >= 0;
		}

		public static bool operator >=(int x, BigInt y)
		{
			if(x == 0)
				return 0 >= y.InternalValue.ChunkCount;

			return y.CompareTo(x) <= 0;
		}

		public static bool operator >=(BigInt x, int y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount >= 0;

			return x.CompareTo(y) >= 0;
		}

		public static bool operator >=(uint x, BigInt y)
		{
			if(x == 0)
				return 0 >= y.InternalValue.ChunkCount;

			return y.CompareTo(x) <= 0;
		}

		public static bool operator >=(BigInt x, uint y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount >= 0;

			return x.CompareTo(y) >= 0;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator >=(long x, BigInt y)
		{
			return (BigInt)x >= y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator >=(BigInt x, long y)
		{
			return x >= (BigInt)y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator >=(ulong x, BigInt y)
		{
			return (BigInt)x >= y;
		}

		// TODO: Implement by accessing the data directly
		public static bool operator >=(BigInt x, ulong y)
		{
			return x >= (BigInt)y;
		}

		public static bool operator >=(float x, BigInt y)
		{
			if(x == 0)
				return 0 >= y.InternalValue.ChunkCount;

			return y.CompareTo(x) <= 0;
		}

		public static bool operator >=(BigInt x, float y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount >= 0;

			return x.CompareTo(y) >= 0;
		}

		public static bool operator >=(double x, BigInt y)
		{
			if(x == 0)
				return 0 >= y.InternalValue.ChunkCount;

			return y.CompareTo(x) <= 0;
		}

		public static bool operator >=(BigInt x, double y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount >= 0;

			return x.CompareTo(y) >= 0;
		}

		public static bool operator >=(decimal x, BigInt y)
		{
			if(x == 0)
				return 0 >= y.InternalValue.ChunkCount;

			return y.CompareTo(x) <= 0;
		}

		public static bool operator >=(BigInt x, decimal y)
		{
			if(y == 0)
				return x.InternalValue.ChunkCount >= 0;

			return x.CompareTo(y) >= 0;
		}

		public static BigInt operator <<(BigInt x, int shiftAmount)
		{
			BigInt z = new BigInt();
			__gmpz_mul_2exp(ref z.InternalValue, ref x.InternalValue, (uint)shiftAmount);
			return z;
		}

		public static BigInt operator >>(BigInt x, int shiftAmount)
		{
			BigInt z = new BigInt();
			__gmpz_tdiv_q_2exp(ref z.InternalValue, ref x.InternalValue, (uint)shiftAmount);
			return z;
		}

		public int this[int bitIndex]
		{
			get
			{
				return __gmpz_tstbit(ref this.InternalValue, (uint)bitIndex);
			}
		}

		public BigInt ChangeBit(int bitIndex, int value)
		{
			BigInt z = new BigInt(this);

			if(value == 0)
				__gmpz_clrbit(ref z.InternalValue, (uint)bitIndex);
			else
				__gmpz_setbit(ref z.InternalValue, (uint)bitIndex);

			return z;
		}

		#endregion

		#region Basic Arithmatic

		public BigInt Negate()
		{
			return -this;
		}

		public BigInt Complement()
		{
			return (~this);
		}

		public BigInt Add(BigInt x)
		{
			return this + x;
		}

		public BigInt Add(int x)
		{
			return this + x;
		}

		public BigInt Add(uint x)
		{
			return this + x;
		}

		public BigInt Subtract(BigInt x)
		{
			return this - x;
		}

		public BigInt Subtract(int x)
		{
			return this - x;
		}

		public BigInt Subtract(uint x)
		{
			return this - x;
		}

		public BigInt Multiply(BigInt x)
		{
			return this * x;
		}

		public BigInt Multiply(int x)
		{
			return this * x;
		}

		public BigInt Multiply(uint x)
		{
			return this * x;
		}

		public BigInt Square()
		{
			return this * this;
		}

		public BigInt Divide(BigInt x)
		{
			return this / x;
		}

		public BigInt Divide(int x)
		{
			return this / x;
		}

		public BigInt Divide(uint x)
		{
			return this / x;
		}

		public BigInt Divide(BigInt x, out BigInt remainder)
		{
			BigInt quotient = new BigInt();
			remainder = new BigInt();
			__gmpz_tdiv_qr(ref quotient.InternalValue, ref remainder.InternalValue, ref this.InternalValue, ref x.InternalValue);
			return quotient;
		}

		public BigInt Divide(int x, out BigInt remainder)
		{
			BigInt quotient = new BigInt();
			remainder = new BigInt();

			if(x >= 0)
			{
				__gmpz_tdiv_qr_ui(ref quotient.InternalValue, ref remainder.InternalValue, ref this.InternalValue, (uint)x);
			}
			else
			{
				__gmpz_tdiv_qr_ui(ref quotient.InternalValue, ref remainder.InternalValue, ref this.InternalValue, (uint)(-x));
				quotient.InternalValue.ChunkCount = -quotient.InternalValue.ChunkCount;
			}

			return quotient;
		}

		public BigInt Divide(int x, out int remainder)
		{
			BigInt quotient = new BigInt();

			if(x >= 0)
			{
				remainder = (int)__gmpz_tdiv_q_ui(ref quotient.InternalValue, ref this.InternalValue, (uint)x);
			}
			else
			{
				remainder = (int)__gmpz_tdiv_q_ui(ref quotient.InternalValue, ref this.InternalValue, (uint)(-x));
				quotient.InternalValue.ChunkCount = -quotient.InternalValue.ChunkCount;
			}

			if(this.InternalValue.ChunkCount < 0)
				remainder = -remainder;

			return quotient;
		}

		public BigInt Divide(uint x, out BigInt remainder)
		{
			BigInt quotient = new BigInt();
			remainder = new BigInt();
			__gmpz_tdiv_qr_ui(ref quotient.InternalValue, ref remainder.InternalValue, ref this.InternalValue, x);
			return quotient;
		}

		public BigInt Divide(uint x, out uint remainder)
		{
			if(this.InternalValue.ChunkCount < 0)
				throw new InvalidOperationException("This method may not be called when the instance represents a negative number.");

			BigInt quotient = new BigInt();
			remainder = __gmpz_tdiv_q_ui(ref quotient.InternalValue, ref this.InternalValue, x);
			return quotient;
		}

		public BigInt Divide(uint x, out int remainder)
		{
			BigInt quotient = new BigInt();
			uint uintRemainder = __gmpz_tdiv_q_ui(ref quotient.InternalValue, ref this.InternalValue, x);
			if(uintRemainder > (uint)int.MaxValue)
				throw new OverflowException();

			if(this.InternalValue.ChunkCount >= 0)
				remainder = (int)uintRemainder;
			else
				remainder = -(int)uintRemainder;

			return quotient;
		}

		public BigInt Remainder(BigInt x)
		{
			BigInt z = new BigInt();
			__gmpz_tdiv_r(ref z.InternalValue, ref this.InternalValue, ref x.InternalValue);
			return z;
		}

		public bool IsDivisibleBy(BigInt x)
		{
			return __gmpz_divisible_p(ref this.InternalValue, ref x.InternalValue) != 0;
		}

		public bool IsDivisibleBy(int x)
		{
			if(x >= 0)
				return __gmpz_divisible_ui_p(ref this.InternalValue, (uint)x) != 0;
			else
				return __gmpz_divisible_ui_p(ref this.InternalValue, (uint)(-x)) != 0;
		}

		public bool IsDivisibleBy(uint x)
		{
			return __gmpz_divisible_ui_p(ref this.InternalValue, x) != 0;
		}

		/// <summary>
		/// Divides exactly. Only works when the division is gauranteed to be exact (there is no remainder).
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public BigInt DivideExactly(BigInt x)
		{
			BigInt z = new BigInt();
			__gmpz_divexact(ref z.InternalValue, ref this.InternalValue, ref x.InternalValue);
			return z;
		}

		public BigInt DivideExactly(int x)
		{
			BigInt z = new BigInt();
			__gmpz_divexact_ui(ref z.InternalValue, ref this.InternalValue, (uint)x);

			if(x < 0)
			{
				// Negate
				z.InternalValue.ChunkCount = -z.InternalValue.ChunkCount;
			}

			return z;
		}

		public BigInt DivideExactly(uint x)
		{
			BigInt z = new BigInt();
			__gmpz_divexact_ui(ref z.InternalValue, ref this.InternalValue, x);
			return z;
		}

		public BigInt DivideMod(BigInt x, BigInt mod)
		{
			return (this * x.InvertMod(mod)) % mod;
		}

		public BigInt And(BigInt x)
		{
			return (this & x);
		}

		public BigInt Or(BigInt x)
		{
			return (this | x);
		}

		public BigInt Xor(BigInt x)
		{
			return (this ^ x);
		}

		public BigInt Mod(BigInt mod)
		{
			return (this % mod);
		}

		public BigInt Mod(int mod)
		{
			return (this % mod);
		}

		public BigInt Mod(uint mod)
		{
			return (this % mod);
		}

		public int ModAsInt32(int mod)
		{
			if(mod < 0)
				throw new ArgumentOutOfRangeException();

			return (int)__gmpz_fdiv_ui(ref this.InternalValue, (uint)mod);
		}

		public uint ModAsUInt32(uint mod)
		{
			return __gmpz_fdiv_ui(ref this.InternalValue, mod);
		}

		public BigInt ShiftLeft(int shiftAmount)
		{
			return (this << shiftAmount);
		}

		public BigInt ShiftRight(int shiftAmount)
		{
			return (this >> shiftAmount);
		}

		public BigInt PowerMod(BigInt exponent, BigInt mod)
		{
			BigInt z = new BigInt();
			__gmpz_powm(ref z.InternalValue, ref this.InternalValue, ref exponent.InternalValue, ref mod.InternalValue);
			return z;
		}

		public BigInt PowerMod(int exponent, BigInt mod)
		{
			BigInt z = new BigInt();
			__gmpz_powm_ui(ref z.InternalValue, ref this.InternalValue, (uint)exponent, ref mod.InternalValue);
			return z;
		}

		public BigInt PowerMod(uint exponent, BigInt mod)
		{
			BigInt z = new BigInt();
			if(exponent >= 0)
			{
				__gmpz_powm_ui(ref z.InternalValue, ref this.InternalValue, exponent, ref mod.InternalValue);
			}
			else
			{
				BigInt bigExponent = exponent;
				BigInt inverse = bigExponent.InvertMod(mod);
				__gmpz_powm_ui(ref z.InternalValue, ref inverse.InternalValue, exponent, ref mod.InternalValue);
			}

			return z;
		}

		public BigInt Power(int exponent)
		{
			if(exponent < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			__gmpz_pow_ui(ref z.InternalValue, ref this.InternalValue, (uint)exponent);
			return z;
		}

		public BigInt Power(uint exponent)
		{
			BigInt z = new BigInt();
			__gmpz_pow_ui(ref z.InternalValue, ref this.InternalValue, exponent);
			return z;
		}

		public static BigInt Power(int x, int exponent)
		{
			if(exponent < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			__gmpz_ui_pow_ui(ref z.InternalValue, (uint)x, (uint)exponent);
			return z;
		}

		public static BigInt Power(uint x, uint exponent)
		{
			BigInt z = new BigInt();
			__gmpz_ui_pow_ui(ref z.InternalValue, x, exponent);
			return z;
		}

		public BigInt InvertMod(BigInt mod)
		{
			BigInt z = new BigInt();
			int status = __gmpz_invert(ref z.InternalValue, ref this.InternalValue, ref mod.InternalValue);
			if(status == 0)
				throw new ArithmeticException("This modular inverse does not exists.");
			return z;
		}

		public bool TryInvertMod(BigInt mod, out BigInt result)
		{
			BigInt z = new BigInt();
			int status = __gmpz_invert(ref z.InternalValue, ref this.InternalValue, ref mod.InternalValue);

			if(status == 0)
			{
				result = null;
				return false;
			}
			else
			{
				result = z;
				return true;
			}
		}

		public bool InverseModExists(BigInt mod)
		{
			BigInt result;
			TryInvertMod(mod, out result);
			return true;
		}

		public bool IsOdd
		{
			get
			{
				if(this.InternalValue.ChunkCount == 0)
					return false;
				else
					return (this.InternalValue.Data[0] & 1) == 1;
			}
		}

		public bool IsEven
		{
			get
			{
				if(this.InternalValue.ChunkCount == 0)
					return true;
				else
					return (this.InternalValue.Data[0] & 1) == 0;
			}
		}

		// TODO: Optimize this by accessing memory directly.
		public int BitLength
		{
			get
			{
				return (int)__gmpz_sizeinbase(ref this.InternalValue, 2);
			}
		}

		#endregion

		#region Roots

		public BigInt Sqrt()
		{
			BigInt z = new BigInt();
			__gmpz_sqrt(ref z.InternalValue, ref this.InternalValue);
			return z;
		}

		public BigInt Sqrt(out BigInt remainder)
		{
			BigInt z = new BigInt();
			remainder = new BigInt();
			__gmpz_sqrtrem(ref z.InternalValue, ref remainder.InternalValue, ref this.InternalValue);
			return z;
		}

		public BigInt Sqrt(out bool isExact)
		{
			BigInt z = new BigInt();
			int result = __gmpz_root(ref z.InternalValue, ref this.InternalValue, 2);
			isExact = result != 0;
			return z;
		}

		public BigInt Root(int n)
		{
			if(n < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			__gmpz_root(ref z.InternalValue, ref this.InternalValue, (uint)n);
			return z;
		}

		public BigInt Root(uint n)
		{
			BigInt z = new BigInt();
			__gmpz_root(ref z.InternalValue, ref this.InternalValue, n);
			return z;
		}

		public BigInt Root(int n, out bool isExact)
		{
			if(n < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			int result = __gmpz_root(ref z.InternalValue, ref this.InternalValue, (uint)n);
			isExact = result != 0;
			return z;
		}

		public BigInt Root(uint n, out bool isExact)
		{
			BigInt z = new BigInt();
			int result = __gmpz_root(ref z.InternalValue, ref this.InternalValue, n);
			isExact = result != 0;
			return z;
		}

		public BigInt Root(int n, out BigInt remainder)
		{
			if(n < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			remainder = new BigInt();
			__gmpz_rootrem(ref z.InternalValue, ref remainder.InternalValue, ref this.InternalValue, (uint)n);
			return z;
		}

		public BigInt Root(uint n, out BigInt remainder)
		{
			BigInt z = new BigInt();
			remainder = new BigInt();
			__gmpz_rootrem(ref z.InternalValue, ref remainder.InternalValue, ref this.InternalValue, n);
			return z;
		}

		public bool IsPerfectSquare()
		{
			return __gmpz_perfect_square_p(ref this.InternalValue) != 0;
		}

		public bool IsPerfectPower()
		{
			// There is a known issue with this function for negative inputs in GMP 4.2.4.
			// See: http://gmplib.org/oldrel/
			if(this.InternalValue.ChunkCount < 0)
				throw new NotImplementedException();

			return __gmpz_perfect_power_p(ref this.InternalValue) != 0;
		}

		#endregion

		#region Number Theoretic Functions

		/// <summary>
		/// Uses the Rabin-Miller primality test to determine if the number is prime.
		/// </summary>
		/// <param name="repetitions"></param>
		/// <returns></returns>
		public bool IsProbablyPrimeRabinMiller(int repetitions)
		{
			int result = __gmpz_probab_prime_p(ref this.InternalValue, repetitions);

			return result != 0;
		}

		// TODO: Create a version of this method which takes in a parameter to represent how well tested the prime should be.
		public BigInt NextPrimeGMP()
		{
			BigInt z = new BigInt();
			__gmpz_nextprime(ref z.InternalValue, ref this.InternalValue);
			return z;
		}

		public static BigInt Gcd(BigInt x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_gcd(ref z.InternalValue, ref x.InternalValue, ref y.InternalValue);
			return z;
		}

		public static BigInt Gcd(BigInt x, int y)
		{
			BigInt z = new BigInt();

			if(y >= 0)
				__gmpz_gcd_ui(ref z.InternalValue, ref x.InternalValue, (uint)y);
			else
				__gmpz_gcd_ui(ref z.InternalValue, ref x.InternalValue, (uint)(-y));

			return z;
		}

		public static BigInt Gcd(int x, BigInt y)
		{
			BigInt z = new BigInt();

			if(x >= 0)
				__gmpz_gcd_ui(ref z.InternalValue, ref y.InternalValue, (uint)x);
			else
				__gmpz_gcd_ui(ref z.InternalValue, ref y.InternalValue, (uint)(-x));

			return z;
		}

		public static BigInt Gcd(BigInt x, uint y)
		{
			BigInt z = new BigInt();
			__gmpz_gcd_ui(ref z.InternalValue, ref x.InternalValue, y);
			return z;
		}

		public static BigInt Gcd(uint x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_gcd_ui(ref z.InternalValue, ref y.InternalValue, x);
			return z;
		}

		public static BigInt Gcd(BigInt x, BigInt y, out BigInt a, out BigInt b)
		{
			BigInt z = new BigInt();
			a = new BigInt();
			b = new BigInt();
			__gmpz_gcdext(ref z.InternalValue, ref a.InternalValue, ref b.InternalValue, ref x.InternalValue, ref y.InternalValue);
			return z;
		}

		public static BigInt Gcd(BigInt x, BigInt y, out BigInt a)
		{
			BigInt z = new BigInt();
			a = new BigInt();
			__gmpz_gcdext(ref z.InternalValue, ref a.InternalValue, null, ref x.InternalValue, ref y.InternalValue);
			return z;
		}

		public static BigInt Lcm(BigInt x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_lcm(ref z.InternalValue, ref x.InternalValue, ref y.InternalValue);
			return z;
		}

		public static BigInt Lcm(BigInt x, int y)
		{
			BigInt z = new BigInt();

			if(y >= 0)
				__gmpz_lcm_ui(ref z.InternalValue, ref x.InternalValue, (uint)y);
			else
				__gmpz_lcm_ui(ref z.InternalValue, ref x.InternalValue, (uint)(-y));

			return z;
		}

		public static BigInt Lcm(int x, BigInt y)
		{
			BigInt z = new BigInt();

			if(x >= 0)
				__gmpz_lcm_ui(ref z.InternalValue, ref y.InternalValue, (uint)x);
			else
				__gmpz_lcm_ui(ref z.InternalValue, ref y.InternalValue, (uint)(-x));

			return z;
		}

		public static BigInt Lcm(BigInt x, uint y)
		{
			BigInt z = new BigInt();
			__gmpz_lcm_ui(ref z.InternalValue, ref x.InternalValue, y);
			return z;
		}

		public static BigInt Lcm(uint x, BigInt y)
		{
			BigInt z = new BigInt();
			__gmpz_lcm_ui(ref z.InternalValue, ref y.InternalValue, x);
			return z;
		}

		public static int LegendreSymbol(BigInt x, BigInt primeY)
		{
			Debug.Assert(primeY != 2); // Not defined for 2

			return __gmpz_jacobi(ref x.InternalValue, ref primeY.InternalValue);
		}

		public static int JacobiSymbol(BigInt x, BigInt y)
		{
			if(y.IsEven || y < 0)
				throw new ArgumentException();

			return __gmpz_jacobi(ref x.InternalValue, ref y.InternalValue);
		}

		public static int JacobiSymbol(BigInt x, int y)
		{
			if((y & 1) == 0 || y < 0)
				throw new ArgumentException();

			return __gmpz_kronecker_si(ref x.InternalValue, y);
		}

		public static int JacobiSymbol(int x, BigInt y)
		{
			if(y.IsEven || y < 0)
				throw new ArgumentException();

			return __gmpz_si_kronecker(x, ref y.InternalValue);
		}

		public static int JacobiSymbol(BigInt x, uint y)
		{
			if((y & 1) == 0)
				throw new ArgumentException();

			return __gmpz_kronecker_ui(ref x.InternalValue, y);
		}

		public static int JacobiSymbol(uint x, BigInt y)
		{
			if(y.IsEven)
				throw new ArgumentException();

			return __gmpz_ui_kronecker(x, ref y.InternalValue);
		}

		public static int KroneckerSymbol(BigInt x, BigInt y)
		{
			return __gmpz_kronecker(ref x.InternalValue, ref y.InternalValue);
		}

		public static int KroneckerSymbol(BigInt x, int y)
		{
			return __gmpz_kronecker_si(ref x.InternalValue, y);
		}

		public static int KroneckerSymbol(int x, BigInt y)
		{
			return __gmpz_si_kronecker(x, ref y.InternalValue);
		}

		public static int KroneckerSymbol(BigInt x, uint y)
		{
			return __gmpz_kronecker_ui(ref x.InternalValue, y);
		}

		public static int KroneckerSymbol(uint x, BigInt y)
		{
			return __gmpz_ui_kronecker(x, ref y.InternalValue);
		}

		public BigInt RemoveFactor(BigInt factor)
		{
			BigInt z = new BigInt();
			__gmpz_remove(ref z.InternalValue, ref this.InternalValue, ref factor.InternalValue);
			return z;
		}

		public BigInt RemoveFactor(BigInt factor, out int count)
		{
			BigInt z = new BigInt();
			count = (int)__gmpz_remove(ref z.InternalValue, ref this.InternalValue, ref factor.InternalValue);
			return z;
		}

		public static BigInt Factorial(int x)
		{
			if(x < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			__gmpz_fac_ui(ref z.InternalValue, (uint)x);
			return z;
		}

		public static BigInt Factorial(uint x)
		{
			BigInt z = new BigInt();
			__gmpz_fac_ui(ref z.InternalValue, x);
			return z;
		}

		public static BigInt Binomial(BigInt n, uint k)
		{
			BigInt z = new BigInt();
			__gmpz_bin_ui(ref z.InternalValue, ref n.InternalValue, k);
			return z;
		}

		public static BigInt Binomial(BigInt n, int k)
		{
			if(k < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			__gmpz_bin_ui(ref z.InternalValue, ref n.InternalValue, (uint)k);
			return z;
		}

		public static BigInt Binomial(uint n, uint k)
		{
			BigInt z = new BigInt();
			__gmpz_bin_uiui(ref z.InternalValue, n, k);
			return z;
		}

		public static BigInt Binomial(int n, int k)
		{
			if(k < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();

			if(n >= 0)
			{
				__gmpz_bin_uiui(ref z.InternalValue, (uint)n, (uint)k);
			}
			else
			{
				// Use the identity bin(n,k) = (-1)^k * bin(-n+k-1,k)
				__gmpz_bin_uiui(ref z.InternalValue, (uint)(-n + k - 1), (uint)k);

				if((k & 1) != 0)
					z.InternalValue.ChunkCount = -z.InternalValue.ChunkCount;
			}

			return z;
		}

		public static BigInt Fibonacci(int n)
		{
			if(n < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			__gmpz_fib_ui(ref z.InternalValue, (uint)n);
			return z;
		}

		public static BigInt Fibonacci(uint n)
		{
			BigInt z = new BigInt();
			__gmpz_fib_ui(ref z.InternalValue, n);
			return z;
		}

		public static BigInt Fibonacci(int n, out BigInt previous)
		{
			if(n < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			previous = new BigInt();
			__gmpz_fib2_ui(ref z.InternalValue, ref previous.InternalValue, (uint)n);
			return z;
		}

		public static BigInt Fibonacci(uint n, out BigInt previous)
		{
			BigInt z = new BigInt();
			previous = new BigInt();
			__gmpz_fib2_ui(ref z.InternalValue, ref previous.InternalValue, n);
			return z;
		}

		public static BigInt Lucas(int n)
		{
			if(n < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			__gmpz_lucnum_ui(ref z.InternalValue, (uint)n);
			return z;
		}

		public static BigInt Lucas(uint n)
		{
			BigInt z = new BigInt();
			__gmpz_lucnum_ui(ref z.InternalValue, n);
			return z;
		}

		public static BigInt Lucas(int n, out BigInt previous)
		{
			if(n < 0)
				throw new ArgumentOutOfRangeException();

			BigInt z = new BigInt();
			previous = new BigInt();
			__gmpz_lucnum2_ui(ref z.InternalValue, ref previous.InternalValue, (uint)n);
			return z;
		}

		public static BigInt Lucas(uint n, out BigInt previous)
		{
			BigInt z = new BigInt();
			previous = new BigInt();
			__gmpz_lucnum2_ui(ref z.InternalValue, ref previous.InternalValue, n);
			return z;
		}

		#endregion

		#region Bitwise Functions

		public int CountOnes()
		{
			return (int)__gmpz_popcount(ref this.InternalValue);
		}

		public int HammingDistance(BigInt x, BigInt y)
		{
			return (int)__gmpz_hamdist(ref x.InternalValue, ref y.InternalValue);
		}

		public int IndexOfZero(int startingIndex)
		{
			unchecked
			{
				if(startingIndex < 0)
					throw new ArgumentOutOfRangeException();

				// Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
				return (int)__gmpz_scan0(ref this.InternalValue, (uint)startingIndex);
			}
		}

		public int IndexOfOne(int startingIndex)
		{
			unchecked
			{
				if(startingIndex < 0)
					throw new ArgumentOutOfRangeException();

				// Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
				return (int)__gmpz_scan1(ref this.InternalValue, (uint)startingIndex);
			}
		}

		#endregion

		#region Comparing

		public unsafe override int GetHashCode()
		{
			uint hash = 0;

			int chunkCount = this.InternalValue.ChunkCount;
			chunkCount = chunkCount >= 0 ? chunkCount : -chunkCount;

			uint* data = this.InternalValue.Data;
			uint* end = data + chunkCount;

			uint* p = data;
			while(p < end)
			{
				hash ^= *p;
				++p;
			}

			return (int)p;
		}

		public bool Equals(BigInt other)
		{
			if(object.ReferenceEquals(other, null))
				return false;

			return Compare(this, other) == 0;
		}

		public override bool Equals(object obj)
		{
			if(object.ReferenceEquals(obj, null))
				return false;

			BigInt objAsBigInt = obj as BigInt;

			if(object.ReferenceEquals(objAsBigInt, null))
			{
				if(obj is int)
					return this == (int)obj;
				else if(obj is uint)
					return this == (uint)obj;
				else if(obj is long)
					return this == (long)obj;
				else if(obj is ulong)
					return this == (ulong)obj;
				else if(obj is double)
					return this == (double)obj;
				else if(obj is float)
					return this == (float)obj;
				else if(obj is short)
					return this == (short)obj;
				else if(obj is ushort)
					return this == (ushort)obj;
				else if(obj is byte)
					return this == (byte)obj;
				else if(obj is sbyte)
					return this == (sbyte)obj;
				else if(obj is decimal)
					return this == (decimal)obj;

				return false;
			}

			return this.CompareTo(objAsBigInt) == 0;
		}

		public bool Equals(int other)
		{
			return this.CompareTo(other) == 0;
		}

		public bool Equals(uint other)
		{
			return this.CompareTo(other) == 0;
		}

		public bool Equals(long other)
		{
			return this.CompareTo(other) == 0;
		}

		public bool Equals(ulong other)
		{
			return this.CompareTo(other) == 0;
		}

		public bool Equals(double other)
		{
			return this.CompareTo(other) == 0;
		}

		public bool Equals(decimal other)
		{
			return this.CompareTo(other) == 0;
		}

		public bool EqualsMod(BigInt x, BigInt mod)
		{
			return __gmpz_congruent_p(ref this.InternalValue, ref x.InternalValue, ref mod.InternalValue) != 0;
		}

		public bool EqualsMod(int x, int mod)
		{
			if(mod < 0)
				throw new ArgumentOutOfRangeException();

			if(x >= 0)
			{
				return __gmpz_congruent_ui_p(ref this.InternalValue, (uint)x, (uint)mod) != 0;
			}
			else
			{
				uint xAsUint = (uint)((x % mod) + mod);
				return __gmpz_congruent_ui_p(ref this.InternalValue, xAsUint, (uint)mod) != 0;
			}
		}

		public bool EqualsMod(uint x, uint mod)
		{
			return __gmpz_congruent_ui_p(ref this.InternalValue, x, mod) != 0;
		}

		public static bool operator ==(BigInt x, BigInt y)
		{
			bool xNull = object.ReferenceEquals(x, null);
			bool yNull = object.ReferenceEquals(y, null);

			if(xNull || yNull)
				return xNull && yNull;

			return x.CompareTo(y) == 0;
		}

		public static bool operator ==(int x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return false;

			if(x == 0)
				return 0 == y.InternalValue.ChunkCount;

			return y.CompareTo(x) == 0;
		}

		public static bool operator ==(BigInt x, int y)
		{
			if(object.ReferenceEquals(x, null))
				return false;

			if(y == 0)
				return x.InternalValue.ChunkCount == 0;

			return x.CompareTo(y) == 0;
		}

		public static bool operator ==(uint x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return false;

			if(x == 0)
				return 0 == y.InternalValue.ChunkCount;

			return y.CompareTo(x) == 0;
		}

		public static bool operator ==(BigInt x, uint y)
		{
			if(object.ReferenceEquals(x, null))
				return false;

			if(y == 0)
				return x.InternalValue.ChunkCount == 0;

			return x.CompareTo(y) == 0;
		}

		// TODO: Optimize this by accessing memory directly.
		public static bool operator ==(long x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return false;

			if(x == 0)
				return 0 == y.InternalValue.ChunkCount;

			return y.CompareTo((BigInt)x) == 0;
		}

		// TODO: Optimize this by accessing memory directly.
		public static bool operator ==(BigInt x, long y)
		{
			if(object.ReferenceEquals(x, null))
				return false;

			if(y == 0)
				return x.InternalValue.ChunkCount == 0;

			return x.CompareTo((BigInt)y) == 0;
		}

		// TODO: Optimize this by accessing memory directly.
		public static bool operator ==(ulong x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return false;

			if(x == 0)
				return 0 == y.InternalValue.ChunkCount;

			return y.CompareTo((BigInt)x) == 0;
		}

		// TODO: Optimize this by accessing memory directly.
		public static bool operator ==(BigInt x, ulong y)
		{
			if(object.ReferenceEquals(x, null))
				return false;

			if(y == 0)
				return x.InternalValue.ChunkCount == 0;

			return x.CompareTo((BigInt)y) == 0;
		}

		public static bool operator ==(float x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return false;

			if(x == 0)
				return 0 == y.InternalValue.ChunkCount;

			return y.CompareTo(x) == 0;
		}

		public static bool operator ==(BigInt x, float y)
		{
			if(object.ReferenceEquals(x, null))
				return false;

			if(y == 0)
				return x.InternalValue.ChunkCount == 0;

			return x.CompareTo(y) == 0;
		}

		public static bool operator ==(double x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return false;

			if(x == 0)
				return 0 == y.InternalValue.ChunkCount;

			return y.CompareTo(x) == 0;
		}

		public static bool operator ==(BigInt x, double y)
		{
			if(object.ReferenceEquals(x, null))
				return false;

			if(y == 0)
				return x.InternalValue.ChunkCount == 0;

			return x.CompareTo(y) == 0;
		}

		public static bool operator ==(decimal x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return false;

			if(x == 0)
				return 0 == y.InternalValue.ChunkCount;

			return y.CompareTo(x) == 0;
		}

		public static bool operator ==(BigInt x, decimal y)
		{
			if(object.ReferenceEquals(x, null))
				return false;

			if(y == 0)
				return x.InternalValue.ChunkCount == 0;

			return x.CompareTo(y) == 0;
		}

		public static bool operator !=(BigInt x, BigInt y)
		{
			bool xNull = object.ReferenceEquals(x, null);
			bool yNull = object.ReferenceEquals(y, null);

			if(xNull || yNull)
				return !(xNull && yNull);

			return x.CompareTo(y) != 0;
		}

		public static bool operator !=(int x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return true;

			if(x == 0)
				return 0 != y.InternalValue.ChunkCount;

			return y.CompareTo(x) != 0;
		}

		public static bool operator !=(BigInt x, int y)
		{
			if(object.ReferenceEquals(x, null))
				return true;

			if(y == 0)
				return x.InternalValue.ChunkCount != 0;

			return x.CompareTo(y) != 0;
		}

		public static bool operator !=(uint x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return true;

			if(x == 0)
				return 0 != y.InternalValue.ChunkCount;

			return y.CompareTo(x) != 0;
		}

		public static bool operator !=(BigInt x, uint y)
		{
			if(object.ReferenceEquals(x, null))
				return true;

			if(y == 0)
				return x.InternalValue.ChunkCount != 0;

			return x.CompareTo(y) != 0;
		}

		// TODO: Optimize this by accessing memory directly
		public static bool operator !=(long x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return true;

			if(x == 0)
				return 0 != y.InternalValue.ChunkCount;

			return y.CompareTo((BigInt)x) != 0;
		}

		// TODO: Optimize this by accessing memory directly
		public static bool operator !=(BigInt x, long y)
		{
			if(object.ReferenceEquals(x, null))
				return true;

			if(y == 0)
				return x.InternalValue.ChunkCount != 0;

			return x.CompareTo((BigInt)y) != 0;
		}

		// TODO: Optimize this by accessing memory directly
		public static bool operator !=(ulong x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return true;

			if(x == 0)
				return 0 != y.InternalValue.ChunkCount;

			return y.CompareTo((BigInt)x) != 0;
		}

		// TODO: Optimize this by accessing memory directly
		public static bool operator !=(BigInt x, ulong y)
		{
			if(object.ReferenceEquals(x, null))
				return true;

			if(y == 0)
				return x.InternalValue.ChunkCount != 0;

			return x.CompareTo((BigInt)y) != 0;
		}

		public static bool operator !=(float x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return true;

			if(x == 0)
				return 0 != y.InternalValue.ChunkCount;

			return y.CompareTo(x) != 0;
		}

		public static bool operator !=(BigInt x, float y)
		{
			if(object.ReferenceEquals(x, null))
				return true;

			if(y == 0)
				return x.InternalValue.ChunkCount != 0;

			return x.CompareTo(y) != 0;
		}

		public static bool operator !=(double x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return true;

			if(x == 0)
				return 0 != y.InternalValue.ChunkCount;

			return y.CompareTo(x) != 0;
		}

		public static bool operator !=(BigInt x, double y)
		{
			if(object.ReferenceEquals(x, null))
				return true;

			if(y == 0)
				return x.InternalValue.ChunkCount != 0;

			return x.CompareTo(y) != 0;
		}

		public static bool operator !=(decimal x, BigInt y)
		{
			if(object.ReferenceEquals(y, null))
				return true;

			if(x == 0)
				return 0 != y.InternalValue.ChunkCount;

			return y.CompareTo(x) != 0;
		}

		public static bool operator !=(BigInt x, decimal y)
		{
			if(object.ReferenceEquals(x, null))
				return true;

			if(y == 0)
				return x.InternalValue.ChunkCount != 0;

			return x.CompareTo(y) != 0;
		}

		public int CompareTo(object obj)
		{
			BigInt objAsBigInt = obj as BigInt;

			if(object.ReferenceEquals(objAsBigInt, null))
			{
				if(obj is int)
					return this.CompareTo((int)obj);
				else if(obj is uint)
					return this.CompareTo((uint)obj);
				else if(obj is long)
					return this.CompareTo((long)obj);
				else if(obj is ulong)
					return this.CompareTo((ulong)obj);
				else if(obj is double)
					return this.CompareTo((double)obj);
				else if(obj is float)
					return this.CompareTo((float)obj);
				else if(obj is short)
					return this.CompareTo((short)obj);
				else if(obj is ushort)
					return this.CompareTo((ushort)obj);
				else if(obj is byte)
					return this.CompareTo((byte)obj);
				else if(obj is sbyte)
					return this.CompareTo((sbyte)obj);
				else if(obj is decimal)
					return this.CompareTo((decimal)obj);
				else if(obj is string)
					return this.CompareTo(new BigInt(obj as string));
				else
					throw new ArgumentException("Cannot compare to " + obj.GetType());
			}

			return this.CompareTo(objAsBigInt);
		}

		public int CompareTo(BigInt other)
		{
			return __gmpz_cmp(ref this.InternalValue, ref other.InternalValue);
		}

		public int CompareTo(int other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount;

			return __gmpz_cmp_si(ref this.InternalValue, other);
		}

		public int CompareTo(uint other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount;

			return __gmpz_cmp_ui(ref this.InternalValue, other);
		}

		// TODO: Optimize by accessing the memory directly
		public int CompareTo(long other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount;

			return this.CompareTo((BigInt)other);
		}

		// TODO: Optimize by accessing the memory directly
		public int CompareTo(ulong other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount;

			return this.CompareTo((BigInt)other);
		}

		public int CompareTo(float other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount;

			return __gmpz_cmp_d(ref this.InternalValue, (double)other);
		}

		public int CompareTo(double other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount;

			return __gmpz_cmp_d(ref this.InternalValue, other);
		}

		public int CompareTo(decimal other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount;

			return __gmpz_cmp_d(ref this.InternalValue, (double)other);
		}

		public int CompareAbsTo(object obj)
		{
			BigInt objAsBigInt = obj as BigInt;

			if(object.ReferenceEquals(objAsBigInt, null))
			{
				if(obj is int)
					return this.CompareAbsTo((int)obj);
				else if(obj is uint)
					return this.CompareAbsTo((uint)obj);
				else if(obj is long)
					return this.CompareAbsTo((long)obj);
				else if(obj is ulong)
					return this.CompareAbsTo((ulong)obj);
				else if(obj is double)
					return this.CompareAbsTo((double)obj);
				else if(obj is float)
					return this.CompareAbsTo((float)obj);
				else if(obj is short)
					return this.CompareAbsTo((short)obj);
				else if(obj is ushort)
					return this.CompareAbsTo((ushort)obj);
				else if(obj is byte)
					return this.CompareAbsTo((byte)obj);
				else if(obj is sbyte)
					return this.CompareAbsTo((sbyte)obj);
				else if(obj is decimal)
					return this.CompareAbsTo((decimal)obj);
				else if(obj is string)
					return this.CompareAbsTo(new BigInt(obj as string));
				else
					throw new ArgumentException("Cannot compare to " + obj.GetType());
			}

			return this.CompareAbsTo(objAsBigInt);
		}

		public int CompareAbsTo(BigInt other)
		{
			return __gmpz_cmpabs(ref this.InternalValue, ref other.InternalValue);
		}

		public int CompareAbsTo(int other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount == 0 ? 0 : 1;

			return __gmpz_cmpabs_ui(ref this.InternalValue, (uint)other);
		}

		public int CompareAbsTo(uint other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount == 0 ? 0 : 1;

			return __gmpz_cmpabs_ui(ref this.InternalValue, other);
		}

		// TODO: Optimize by accessing the memory directly
		public int CompareAbsTo(long other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount == 0 ? 0 : 1;

			return this.CompareTo((BigInt)other);
		}

		// TODO: Optimize by accessing the memory directly
		public int CompareAbsTo(ulong other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount == 0 ? 0 : 1;

			return this.CompareTo((BigInt)other);
		}

		public int CompareAbsTo(double other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount == 0 ? 0 : 1;

			return __gmpz_cmpabs_d(ref this.InternalValue, other);
		}

		public int CompareAbsTo(decimal other)
		{
			if(other == 0)
				return this.InternalValue.ChunkCount == 0 ? 0 : 1;

			return __gmpz_cmpabs_d(ref this.InternalValue, (double)other);
		}

		public static int Compare(BigInt x, object y)
		{
			return x.CompareTo(y);
		}

		public static int Compare(object x, BigInt y)
		{
			return -y.CompareTo(x);
		}

		public static int Compare(BigInt x, BigInt y)
		{
			return x.CompareTo(y);
		}

		public static int Compare(BigInt x, int y)
		{
			return x.CompareTo(y);
		}

		public static int Compare(int x, BigInt y)
		{
			return -y.CompareTo(x);
		}

		public static int Compare(BigInt x, uint y)
		{
			return x.CompareTo(y);
		}

		public static int Compare(uint x, BigInt y)
		{
			return -y.CompareTo(x);
		}

		public static int Compare(BigInt x, long y)
		{
			return x.CompareTo(y);
		}

		public static int Compare(long x, BigInt y)
		{
			return -y.CompareTo(x);
		}

		public static int Compare(BigInt x, ulong y)
		{
			return x.CompareTo(y);
		}

		public static int Compare(ulong x, BigInt y)
		{
			return -y.CompareTo(x);
		}

		public static int Compare(BigInt x, double y)
		{
			return x.CompareTo(y);
		}

		public static int Compare(double x, BigInt y)
		{
			return -y.CompareTo(x);
		}

		public static int Compare(BigInt x, decimal y)
		{
			return x.CompareTo(y);
		}

		public static int Compare(decimal x, BigInt y)
		{
			return -y.CompareTo(x);
		}

		public static int CompareAbs(BigInt x, object y)
		{
			return x.CompareAbsTo(y);
		}

		public static int CompareAbs(object x, BigInt y)
		{
			return -y.CompareAbsTo(x);
		}

		public static int CompareAbs(BigInt x, BigInt y)
		{
			return x.CompareAbsTo(y);
		}

		public static int CompareAbs(BigInt x, int y)
		{
			return x.CompareAbsTo(y);
		}

		public static int CompareAbs(int x, BigInt y)
		{
			return -y.CompareAbsTo(x);
		}

		public static int CompareAbs(BigInt x, uint y)
		{
			return x.CompareAbsTo(y);
		}

		public static int CompareAbs(uint x, BigInt y)
		{
			return -y.CompareAbsTo(x);
		}

		public static int CompareAbs(BigInt x, long y)
		{
			return x.CompareAbsTo(y);
		}

		public static int CompareAbs(long x, BigInt y)
		{
			return -y.CompareAbsTo(x);
		}

		public static int CompareAbs(BigInt x, ulong y)
		{
			return x.CompareAbsTo(y);
		}

		public static int CompareAbs(ulong x, BigInt y)
		{
			return -y.CompareAbsTo(x);
		}

		public static int CompareAbs(BigInt x, double y)
		{
			return x.CompareAbsTo(y);
		}

		public static int CompareAbs(double x, BigInt y)
		{
			return -y.CompareAbsTo(x);
		}

		public static int CompareAbs(BigInt x, decimal y)
		{
			return x.CompareAbsTo(y);
		}

		public static int CompareAbs(decimal x, BigInt y)
		{
			return -y.CompareAbsTo(x);
		}

		#endregion

		#region Casting

		public static implicit operator BigInt(byte value)
		{
			return new BigInt((uint)value);
		}

		public static implicit operator BigInt(int value)
		{
			if(value <= 10 && value >= -10)
			{
				switch(value)
				{
					case 0:
						return BigInt.Zero;
					case 1:
						return BigInt.One;
					case 2:
						return BigInt.Two;
					case 3:
						return BigInt.Three;
					case 10:
						return BigInt.Ten;
					case -1:
						return BigInt.NegativeOne;
					case -2:
						return BigInt.NegativeTwo;
					case -3:
						return BigInt.Three;
					case -10:
						return BigInt.NegativeTen;
				}
			}

			return new BigInt(value);
		}

		public static implicit operator BigInt(uint value)
		{
			if(value <= 10)
			{
				switch(value)
				{
					case 0:
						return BigInt.Zero;
					case 1:
						return BigInt.One;
					case 2:
						return BigInt.Two;
					case 10:
						return BigInt.Ten;
				}
			}

			return new BigInt(value);
		}

		public static implicit operator BigInt(short value)
		{
			return new BigInt(value);
		}

		public static implicit operator BigInt(ushort value)
		{
			return new BigInt(value);
		}

		public static implicit operator BigInt(long value)
		{
			return new BigInt(value);
		}

		public static implicit operator BigInt(ulong value)
		{
			return new BigInt(value);
		}

		public static implicit operator BigInt(float value)
		{
			return new BigInt((double)value);
		}

		public static implicit operator BigInt(double value)
		{
			return new BigInt(value);
		}

		public static implicit operator BigInt(decimal value)
		{
			return new BigInt(value);
		}

		public static explicit operator BigInt(string value)
		{
			return new BigInt(value);
		}

		public static explicit operator byte(BigInt value)
		{
			return (byte)(uint)value;
		}

		public static explicit operator int(BigInt value)
		{
			unchecked
			{
				int chunkCount = value.InternalValue.ChunkCount;
				uint* data;

				if(chunkCount == 0)
				{
					return 0;
				}
				else if(chunkCount > 0)
				{
					data = value.InternalValue.Data;
					return (int)data[0];
				}
				else
				{
					data = value.InternalValue.Data;
					return -(int)data[0];
				}
			}
		}

		public static explicit operator uint(BigInt value)
		{
			unchecked
			{
				int chunkCount = value.InternalValue.ChunkCount;
				uint* data;

				if(chunkCount == 0)
				{
					return 0;
				}
				else if(chunkCount > 0)
				{
					data = value.InternalValue.Data;
					return (uint)data[0];
				}
				else
				{
					data = value.InternalValue.Data;
					return 0U - (uint)data[0];
				}
			}
		}

		public static explicit operator short(BigInt value)
		{
			return (short)(int)value;
		}

		public static explicit operator ushort(BigInt value)
		{
			return (ushort)(uint)value;
		}

		public static explicit operator long(BigInt value)
		{
			unchecked
			{
				int chunkCount = value.InternalValue.ChunkCount;

				uint* data;

				if(chunkCount == 0)
				{
					return 0;
				}
				else if(chunkCount > 0)
				{
					if(chunkCount == 1)
					{
						data = value.InternalValue.Data;
						return (long)data[0];
					}
					else
					{
						data = value.InternalValue.Data;
						return (long)((ulong)data[0] | (((ulong)data[1]) << 32));
					}
				}
				else // chunkCount < 0
				{
					if(chunkCount == -1)
					{
						data = value.InternalValue.Data;
						return -(long)data[0];
					}
					else
					{
						data = value.InternalValue.Data;
						return -(long)((ulong)data[0] | (((ulong)data[1]) << 32));
					}
				}
			}
		}

		public static explicit operator ulong(BigInt value)
		{
			unchecked
			{
				int size = value.InternalValue.ChunkCount;
				uint* data;

				if(size == 0)
				{
					return 0;
				}
				else if(size > 0)
				{
					if(size == 1)
					{
						data = value.InternalValue.Data;
						return (ulong)data[0];
					}
					else
					{
						data = value.InternalValue.Data;
						return ((ulong)data[0] | (((ulong)data[1]) << 32));
					}
				}
				else
				{
					if(size == -1)
					{
						data = value.InternalValue.Data;
						return 0UL - (ulong)data[0];
					}
					else
					{
						data = value.InternalValue.Data;
						return 0UL - ((ulong)data[0] | (((ulong)data[1]) << 32));
					}
				}
			}
		}

		public static explicit operator float(BigInt value)
		{
			return (float)(double)value;
		}

		public static explicit operator double(BigInt value)
		{
			return __gmpz_get_d(ref value.InternalValue);
		}

		public static explicit operator decimal(BigInt value)
		{
			return (decimal)(double)value;
		}

		public static explicit operator string(BigInt value)
		{
			return value.ToString();
		}

		#endregion

		#region Cloning

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public BigInt Clone()
		{
			return new BigInt(this);
		}

		#endregion

		#region Converstions

		public byte[] ToByteArray()
		{
			byte[] data;

			if(this.InternalValue.ChunkCount == 0)
			{
				data = new byte[0];
			}
			else
			{
				uint size = (__gmpz_sizeinbase(ref this.InternalValue, 2) + 8 - 1) / 8;
				data = new byte[size];

				fixed(byte* dataPtr = data)
					__gmpz_export(dataPtr, null, -1, sizeof(byte), 0, 0, ref this.InternalValue);
			}

			return data;
		}

		public byte[] ToByteArray(out int sign)
		{
			int chunkCount = this.InternalValue.ChunkCount;

			byte[] data;

			if(chunkCount == 0)
			{
				data = new byte[0];
				sign = 0;
			}
			else if(chunkCount > 0)
			{
				uint size = (__gmpz_sizeinbase(ref this.InternalValue, 2) + 8 - 1) / 8;
				data = new byte[size];

				fixed(byte* dataPtr = data)
					__gmpz_export(dataPtr, null, -1, sizeof(byte), 0, 0, ref this.InternalValue);

				sign = 1;
			}
			else
			{
				uint size = (__gmpz_sizeinbase(ref this.InternalValue, 2) + 8 - 1) / 8;
				data = new byte[size];

				fixed(byte* dataPtr = data)
					__gmpz_export(dataPtr, null, -1, sizeof(byte), 0, 0, ref this.InternalValue);

				sign = -1;
			}

			return data;
		}

		public uint[] ToUIntArray()
		{
			uint[] data;

			if(this.InternalValue.ChunkCount == 0)
			{
				data = new uint[0];
			}
			else
			{
				uint size = (__gmpz_sizeinbase(ref this.InternalValue, 2) + 32 - 1) / 32;
				data = new uint[size];

				fixed(uint* dataPtr = data)
					__gmpz_export(dataPtr, null, -1, sizeof(uint), 0, 0, ref this.InternalValue);
			}

			return data;
		}

		public uint[] ToUIntArray(out int sign)
		{
			int chunkCount = this.InternalValue.ChunkCount;

			uint[] data;

			if(chunkCount == 0)
			{
				data = new uint[0];
				sign = 0;
			}
			else if(chunkCount > 0)
			{
				uint size = (__gmpz_sizeinbase(ref this.InternalValue, 2) + 32 - 1) / 32;
				data = new uint[size];

				uint countp = 0;

				fixed(uint* dataPtr = data)
					__gmpz_export(dataPtr, &countp, -1, sizeof(uint), 0, 0, ref this.InternalValue);

				Debug.Assert(countp == size, "countp = " + countp + ", size = " + size);

				sign = 1;
			}
			else
			{
				uint size = (__gmpz_sizeinbase(ref this.InternalValue, 2) + 32 - 1) / 32;
				data = new uint[size];

				uint countp = 0;

				fixed(uint* dataPtr = data)
					__gmpz_export(dataPtr, &countp, -1, sizeof(uint), 0, 0, ref this.InternalValue);

				Debug.Assert(countp == size, "countp = " + countp + ", size = " + size);

				sign = -1;
			}

			return data;
		}

		public override string ToString()
		{
			return ToString(s_defaultStringBase);
		}

		public string ToString(int @base)
		{
			if(@base > -2 && @base < 2 || @base > 62 || @base < -36)
				throw new ArgumentOutOfRangeException();

			int sizeInBase = (int)__gmpz_sizeinbase(ref this.InternalValue, @base);

			StringBuilder s = new StringBuilder(sizeInBase + 2);
			int baseParameter = (@base > 0 && @base <= 36) ? -@base : @base;
			__gmpz_get_str(s, baseParameter, ref this.InternalValue);

			return s.ToString();
		}

		#region IConvertible Members

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return (byte)this;
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return (decimal)this;
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return (double)this;
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return (short)this;
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return (int)this;
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return (long)this;
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return (sbyte)this;
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return (float)this;
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return this.ToString();
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if(targetType == null)
				throw new ArgumentNullException("targetType");

			if(targetType == typeof(BigInt))
				return this;

			IConvertible value = this;

			if(targetType == typeof(sbyte))
			{
				return value.ToSByte(provider);
			}
			if(targetType == typeof(byte))
			{
				return value.ToByte(provider);
			}
			if(targetType == typeof(short))
			{
				return value.ToInt16(provider);
			}
			if(targetType == typeof(ushort))
			{
				return value.ToUInt16(provider);
			}
			if(targetType == typeof(int))
			{
				return value.ToInt32(provider);
			}
			if(targetType == typeof(uint))
			{
				return value.ToUInt32(provider);
			}
			if(targetType == typeof(long))
			{
				return value.ToInt64(provider);
			}
			if(targetType == typeof(ulong))
			{
				return value.ToUInt64(provider);
			}
			if(targetType == typeof(float))
			{
				return value.ToSingle(provider);
			}
			if(targetType == typeof(double))
			{
				return value.ToDouble(provider);
			}
			if(targetType == typeof(decimal))
			{
				return value.ToDecimal(provider);
			}
			if(targetType == typeof(string))
			{
				return value.ToString(provider);
			}
			if(targetType == typeof(object))
			{
				return value;
			}

			throw new InvalidCastException();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return (ushort)this;
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return (uint)this;
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return (ulong)this;
		}

		#endregion

		#endregion Conversions

		#region Helpers

		private static unsafe bool IsZero(uint[] x)
		{
			fixed(uint* aStart = x)
			{
				uint* aEnd = aStart + x.Length;

				uint* a = aStart;

				while(a < aEnd)
				{
					if(*a != 0)
						return false;

					a++;
				}

				return true;
			}
		}

		// TODO: This should be optimized to use uint* instead of byte*.
		private static unsafe bool IsZero(byte[] x)
		{
			fixed(byte* aStart = x)
			{
				byte* aEnd = aStart + x.Length;

				byte* a = aStart;

				while(a < aEnd)
				{
					if(*a != 0)
						return false;

					a++;
				}

				return true;
			}
		}

		#endregion
	}
}
